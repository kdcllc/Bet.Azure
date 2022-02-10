using System.Collections.Concurrent;

using Azure.Identity;
using Azure.Messaging.ServiceBus;

using Bet.Azure.Messaging.Exceptions;
using Bet.Azure.Messaging.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bet.Azure.Messaging;

public class AzureServiceBusConnection : IAzureServiceBusConnection
{
    private readonly IOptionsMonitor<AzureServiceBusOptions> _optionsMonitor;
    private readonly ILogger<AzureServiceBusConnection> _logger;
    private readonly ConcurrentDictionary<string, ServiceBusClient> _store = new();

    private bool _disposed;

    public AzureServiceBusConnection(
        IOptionsMonitor<AzureServiceBusOptions> optionsMonitor,
        ILogger<AzureServiceBusConnection> logger)
    {
        _optionsMonitor = optionsMonitor ?? throw new System.ArgumentNullException(nameof(optionsMonitor));
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    public ServiceBusClient CreateClient(string name)
    {
        var options = _optionsMonitor.Get(name);

        if (_store.TryGetValue(name, out var client))
        {
            if (!client.IsClosed)
            {
                _logger.LogInformation($"[{nameof(AzureServiceBusConnection)}] was reused from cache {nameof(ServiceBusClient)}");
                return client;
            }
            else
            {
                _logger.LogInformation($"[{nameof(AzureServiceBusConnection)}] was closed {nameof(ServiceBusClient)}");
            }
        }

        var newClient = GetServiceBusClient(options, name);
        _store.AddOrUpdate(name, newClient, (_, a) => a);
        return newClient;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return default;
        }

        foreach (var client in _store)
        {
            client.Value.DisposeAsync();
        }

        _disposed = true;
        return default;
    }

    private ServiceBusClient GetServiceBusClient(AzureServiceBusOptions options, string name)
    {
        var busOptions = new ServiceBusClientOptions();
        options.ConfigureServiceBusClientOptions?.Invoke(busOptions);

        if (!string.IsNullOrEmpty(options.ConnectionString))
        {
            _logger.LogInformation($"[{nameof(AzureServiceBusConnection)}] was created for {name} from ConnectionString", name);
            return new ServiceBusClient(options.ConnectionString, busOptions);
        }

        if (!string.IsNullOrEmpty(options.FullyQualifiedNamespace))
        {
            _logger.LogInformation($"[{nameof(AzureServiceBusConnection)}] was created for {name} from {nameof(DefaultAzureCredential)}", name);
            return new ServiceBusClient(options.FullyQualifiedNamespace, new DefaultAzureCredential());
        }

        throw new AzureServiceBusException($"FullyQualifiedNamespace is empty for {name}");
    }
}
