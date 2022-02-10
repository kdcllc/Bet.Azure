using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

using Azure.Messaging.ServiceBus;

using Bet.Azure.Messaging.Internal;
using Bet.BuildingBlocks.Messaging.Abstractions;
using Bet.BuildingBlocks.Messaging.Abstractions.Consumer;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bet.Azure.Messaging;

public class AzureConsumerPool : IAzureConsumerPool
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAzureServiceBusConnection _connection;
    private readonly AzureMessagingServiceBuilder _serviceBuilder;
    private readonly ILogger<AzureConsumerPool> _logger;

    private readonly ConcurrentDictionary<MessageConsumerInfo, ServiceBusProcessor> _processors = new();

    public AzureConsumerPool(
        IServiceProvider serviceProvider,
        IAzureServiceBusConnection connection,
        AzureMessagingServiceBuilder serviceBuilder,
        ILogger<AzureConsumerPool> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _serviceBuilder = serviceBuilder ?? throw new ArgumentNullException(nameof(serviceBuilder));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string Named => _serviceBuilder.Named;

    public Task StartAsync<TModel, THandler>(CancellationToken cancellationToken = default)
          where TModel : EventMessage
          where THandler : IMessageConsumerHandler<TModel>
    {
        var clients = _serviceBuilder.ConsumerMappings.Select(x => x.Value.First(p => p.MessageName == typeof(TModel).Name && p.Handler == typeof(THandler)));
        return RegisterClientAsync(clients, cancellationToken);
    }

    public Task StartAsync<THandler>(CancellationToken cancellationToken = default) where THandler : IDynamicMessageConsumerHandler
    {
        var clients = _serviceBuilder.ConsumerMappings.Select(x => x.Value.First(p => p.Handler == typeof(THandler)));
        return RegisterClientAsync(clients, cancellationToken);
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        var clients = _serviceBuilder.ConsumerMappings.SelectMany(x => x.Value);
        return RegisterClientAsync(clients, cancellationToken);
    }

    public async Task StopAsync<TModel, THandler>(CancellationToken cancellationToken = default)
        where TModel : EventMessage
        where THandler : IMessageConsumerHandler<TModel>
    {
        var clients = _serviceBuilder.ConsumerMappings.Select(x => x.Value.First(p => p.MessageName == typeof(TModel).Name && p.Handler == typeof(THandler)));

        await StopAsync(clients, cancellationToken);
    }

    public async Task StopAsync<THandler>(CancellationToken cancellationToken = default) where THandler : IDynamicMessageConsumerHandler
    {
        var clients = _serviceBuilder.ConsumerMappings.Select(x => x.Value.First(p => p.Handler == typeof(THandler)));

        await StopAsync(clients, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var pro = _processors.Values;
        foreach (var processor in pro)
        {
            if (processor.IsProcessing)
            {
                await processor.StopProcessingAsync(cancellationToken);
                await processor.CloseAsync(cancellationToken);
            }
        }
    }

    private async Task RegisterClientAsync(IEnumerable<MessageConsumerInfo> clients, CancellationToken cancellationToken)
    {
        foreach (var clientName in clients)
        {
            var client = _connection.CreateClient(clientName.Named);

            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 10
            };

            ServiceBusProcessor processor;

            if (!string.IsNullOrEmpty(clientName.Subscription))
            {
                processor = client.CreateProcessor(clientName.QueueOrTopicName, clientName.Subscription, options);
            }
            else
            {
                processor = client.CreateProcessor(clientName.QueueOrTopicName, options);
            }

            processor.ProcessMessageAsync += async (args) =>
            {
                var messageData = Encoding.UTF8.GetString(args.Message.Body.ToArray());

                var messageName = args.Message.ApplicationProperties["messageName"] as string;

                if (await ProcessEventAsync(messageName, messageData, cancellationToken))
                {
                    // on successful processing by handler settle the message
                    await args.CompleteMessageAsync(args.Message, cancellationToken);
                }
            };

            processor.ProcessErrorAsync += async (args) =>
            {
                // the error source tells me at what point in the processing an error occurred
                var exMsg = $"{args.ErrorSource}-{args.FullyQualifiedNamespace}-{args.EntityPath}";

                _logger.LogError(args.Exception, "Error Handling message: {messageException} and tracing information: {trace}", args.Exception.Message, exMsg);

                await Task.CompletedTask;
            };

            if (!processor.IsProcessing)
            {
                // start processing
                await processor.StartProcessingAsync();
            }

            _processors.TryAdd(clientName, processor);
        }
    }

    private async Task<bool> ProcessEventAsync(string? messageName, string message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(messageName))
        {
            return false;
        }

        if (_serviceBuilder.ConsumerPool.IsRegistered(messageName!))
        {
            using var scope = _serviceProvider.CreateScope();

            var messageHandlers = _serviceBuilder.ConsumerPool.GetHandlers(messageName!);

            foreach (var messageHandler in messageHandlers)
            {
                if (messageHandler.IsDynamic)
                {
                    if (scope.ServiceProvider.GetService(messageHandler.HandlerType) is not IDynamicMessageConsumerHandler handler)
                    {
                        continue;
                    }

                    using dynamic eventData = JsonDocument.Parse(message);
                    await handler?.HandleAsync(eventData, cancellationToken);
                }
                else
                {
                    var handler = scope.ServiceProvider.GetService(messageHandler.HandlerType);
                    if (handler == null)
                    {
                        continue;
                    }

                    var eventType = _serviceBuilder.ConsumerPool.GeTypeByName(messageName!);
                    var data = JsonSerializer.Deserialize(message, eventType);
                    var concreteType = typeof(IMessageConsumerHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("HandleAsync").Invoke(handler, new object[] { data!, cancellationToken });
                }
            }

            return true;
        }

        return false;
    }

    private async Task StopAsync(IEnumerable<MessageConsumerInfo> clients, CancellationToken cancellationToken)
    {
        foreach (var client in clients)
        {
            if (_processors.TryGetValue(client, out var processor))
            {
                if (processor.IsProcessing)
                {
                    await processor.StopProcessingAsync(cancellationToken);
                    await processor.CloseAsync(cancellationToken);
                }
            }
        }
    }
}
