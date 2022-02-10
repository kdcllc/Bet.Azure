using Bet.BuildingBlocks.Messaging.Abstractions.Consumer;

namespace Bet.Azure.Messaging.Sample.EventHandlers;

public class BagMessageHandler : IMessageConsumerHandler<BagMessage>
{
    private readonly ILogger<BagMessageHandler> _logger;

    public BagMessageHandler(ILogger<BagMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(BagMessage @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Message Received");

        _logger.LogInformation(@event.Data);

        return Task.CompletedTask;
    }
}
