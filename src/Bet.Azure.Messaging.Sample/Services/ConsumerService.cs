using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.ServiceBus;

using Bet.Azure.Messaging.Sample.EventMessages;
using Bet.Azure.Messaging.Sample.Handlers;

using Microsoft.Extensions.Logging;

namespace Bet.Azure.Messaging.Sample.Services
{
    public class ConsumerService
    {
        private readonly IEnumerable<IAzureConsumerPool> _consumerPools;
        private readonly IAzureServiceBusConnection _connections;
        private readonly ILogger<ConsumerService> _logger;

        public ConsumerService(
            IEnumerable<IAzureConsumerPool> consumerPools,
            IAzureServiceBusConnection connections,
            ILogger<ConsumerService> logger)
        {
            _consumerPools = consumerPools;
            _connections = connections;
            _logger = logger;
        }

        public Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            var queuesOnly = _consumerPools.First(x => x.Named == ServiceBusNames.QueuesOnly);

            return queuesOnly.StartAsync<SendGridEventMessage, SendGridHandler>(cancellationToken);
        }

        public Task ProcessTopicAsync(CancellationToken cancellationToken)
        {
            var topics = _consumerPools.First(x => x.Named == ServiceBusNames.TopicSubscriptions);
            return topics.StartAsync<UspsHandler>(cancellationToken);
        }

        public async Task ProcessorAsync(CancellationToken cancellationToken)
        {
            // 1. named value of the connection
            var client = _connections.CreateClient(ServiceBusNames.QueuesOnly);

            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1
            };

            // 2. Topic or Queue Name
            var processor = client.CreateProcessor("sendgridwebhook_dev", options);

            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            async Task MessageHandler(ProcessMessageEventArgs args)
            {
                var body = args.Message.Body.ToString();
                _logger.LogInformation(body);

                // we can evaluate application logic and use that to determine how to settle the message.
                await args.CompleteMessageAsync(args.Message);
            }

            Task ErrorHandler(ProcessErrorEventArgs args)
            {
                // the error source tells me at what point in the processing an error occurred
                _logger.LogInformation(args.ErrorSource.ToString());
                // the fully qualified namespace is available
                _logger.LogInformation(args.FullyQualifiedNamespace);
                // as well as the entity path
                _logger.LogInformation(args.EntityPath);
                _logger.LogInformation(args.Exception.ToString());
                return Task.CompletedTask;
            }

            // start processing
            await processor.StartProcessingAsync();
        }

        public async Task PeekMessagesAsync(CancellationToken cancellationToken)
        {
            // Azure Service Bus Data Receiver
            var options = new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            };

            var client = _connections.CreateClient(ServiceBusNames.QueuesOnly);

            await using var receiver = client.CreateReceiver("sendgridwebhook_dev", options);

            _logger.LogInformation($"{DateTime.Now} :: Receiving Messages sendgridwebhooks");
            var receivedMessageCount = 0;
            while (true)
            {
                var receivedMessage = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(1));
                if (receivedMessage != null)
                {
                    var o = receivedMessage.Body.ToObjectFromJson<SendGridEventMessage>();

                    _logger.LogInformation(o.Data);
                    receivedMessageCount++;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
