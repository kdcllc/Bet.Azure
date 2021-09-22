using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bet.Azure.Messaging.Internal
{
    internal class ConsumerHostedService : IHostedService
    {
        private readonly IEnumerable<IAzureConsumerPool> _consumerPools;
        private readonly ILogger<ConsumerHostedService> _logger;

        public ConsumerHostedService(
            IEnumerable<IAzureConsumerPool> consumerPools,
            ILogger<ConsumerHostedService> logger)
        {
            _consumerPools = consumerPools ?? throw new ArgumentNullException(nameof(consumerPools));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var pool in _consumerPools)
            {
                if (pool != null)
                {
                    _logger.LogInformation("Starting {poolName}", pool.Named);
                    await pool.StartAsync(cancellationToken);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var pool in _consumerPools)
            {
                if (pool != null)
                {
                    _logger.LogInformation("Stopping {poolName}", pool.Named);
                    await pool.StopAsync(cancellationToken);
                }
            }
        }
    }
}
