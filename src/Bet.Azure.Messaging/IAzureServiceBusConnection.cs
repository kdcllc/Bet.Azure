using Azure.Messaging.ServiceBus;

namespace Bet.Azure.Messaging;

public interface IAzureServiceBusConnection : IDisposable, IAsyncDisposable
{
    ServiceBusClient CreateClient(string named);
}
