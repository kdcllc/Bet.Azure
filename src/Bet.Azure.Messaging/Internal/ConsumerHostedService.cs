using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bet.Azure.Messaging.Internal
{
    internal class ConsumerHostedService : IHostedService
    {
        private readonly IAzureConsumerPool _pool;
        private readonly ILogger<ConsumerHostedService> _logger;

        public ConsumerHostedService(
            IAzureConsumerPool pool,
            ILogger<ConsumerHostedService> logger)
        {
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting {poolName}", _pool.Named);
            await _pool.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping {poolName}", _pool.Named);
            await _pool.StopAsync(cancellationToken);
        }
    }
}
