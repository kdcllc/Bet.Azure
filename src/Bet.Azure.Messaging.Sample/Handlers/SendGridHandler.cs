using System.Threading;
using System.Threading.Tasks;

using Bet.Azure.Messaging.Sample.EventMessages;
using Bet.BuildingBlocks.Messaging.Abstractions.Consumer;

using Microsoft.Extensions.Logging;

namespace Bet.Azure.Messaging.Sample.Handlers
{
    public class SendGridHandler : IMessageConsumerHandler<SendGridEventMessage>
    {
        private readonly ILogger<SendGridHandler> _logger;

        public SendGridHandler(ILogger<SendGridHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(SendGridEventMessage @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Message Received");

            _logger.LogInformation(@event.Data);

            return Task.CompletedTask;
        }
    }
}
