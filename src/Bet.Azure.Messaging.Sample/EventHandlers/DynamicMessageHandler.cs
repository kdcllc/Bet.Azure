using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Bet.BuildingBlocks.Messaging.Abstractions.Consumer;

using Microsoft.Extensions.Logging;

namespace Bet.Azure.Messaging.Sample.EventHandlers
{
    public class DynamicMessageHandler : IDynamicMessageConsumerHandler
    {
        private readonly ILogger<DynamicMessageHandler> _logger;

        public DynamicMessageHandler(ILogger<DynamicMessageHandler> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public Task HandleAsync(dynamic message, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(message);

            _logger.LogInformation($"Received: {json}");

            if (JsonSerializer.Deserialize<DynamicMessage>(json) is DynamicMessage model)
            {
                _logger.LogInformation(model.OrderId);
            }

            // uncomment this to test error handing routing
            // throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
