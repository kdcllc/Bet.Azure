using Bet.Azure.UnitTest.Messages;
using Bet.BuildingBlocks.Messaging.Abstractions.Consumer;

using Microsoft.Extensions.Logging;

namespace Bet.Azure.UnitTest.EventHandlers;

public class TypedEventMessageHandler : IMessageConsumerHandler<TypedEventMessage>
{
    private readonly ILogger<TypedEventMessageHandler> _logger;

    public TypedEventMessageHandler(ILogger<TypedEventMessageHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task HandleAsync(TypedEventMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Message Received");

        _logger.LogInformation(message.Data);

        return Task.CompletedTask;
    }
}
