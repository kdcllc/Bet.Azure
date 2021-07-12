using System;
using System.Threading;
using System.Threading.Tasks;

using Bet.BuildingBlocks.Messaging.Abstractions.Consumer;

using Microsoft.Extensions.Logging;

namespace Bet.Azure.Messaging.Sample.Handlers
{
    public class UspsHandler : IDynamicMessageConsumerHandler
    {
        private readonly ILogger<UspsHandler> _logger;

        public UspsHandler(ILogger<UspsHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(dynamic message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received Dynamic Message");

            // uncomment this to test error handing routing
            // throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
