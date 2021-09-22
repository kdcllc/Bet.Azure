using System;
using System.Threading.Tasks;

using Bet.Azure.Messaging.Sample.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bet.Azure.Messaging.Sample
{
    public class Main : IMain
    {
        private readonly ILogger<Main> _logger;
        private readonly ProducerService _producerService;
        private readonly ConsumerService _consumerService;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public Main(
            ProducerService producerService,
            ConsumerService consumerService,
            IHostApplicationLifetime applicationLifetime,
            IConfiguration configuration,
            ILogger<Main> logger)
        {
            _producerService = producerService;
            _consumerService = consumerService;
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IConfiguration Configuration { get; set; }

        public async Task<int> RunAsync()
        {
            var token = _applicationLifetime.ApplicationStopping;

            _logger.LogInformation("Main executed");

            // send to a queue
            //await _producerService.SendToQueueAsync(token);
            //await _consumerService.StartReceivingQueueAsync(token);
            //await Task.Delay(TimeSpan.FromSeconds(5), token);
            //await _consumerService.StopReceivingQueueAsync(token);

            // send to a topic
            await _producerService.SendToTopic(token);
            await _consumerService.StartReceivingTopicAsync(token);
            await Task.Delay(TimeSpan.FromSeconds(5), token);
            await _consumerService.StopReceivingTopicAsync(token);

            // use this token for stopping the services
            token.ThrowIfCancellationRequested();

            return 0;
        }
    }
}
