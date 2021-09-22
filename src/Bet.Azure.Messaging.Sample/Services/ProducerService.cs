using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Bet.Azure.Messaging.Sample.EventMessages;

using Microsoft.Extensions.Logging;

namespace Bet.Azure.Messaging.Sample.Services
{
    public class ProducerService
    {
        private readonly IEnumerable<IAzureProducerPool> _producers;
        private readonly ILogger<ProducerService> _logger;

        public ProducerService(
            IEnumerable<IAzureProducerPool> producers,
            ILogger<ProducerService> logger)
        {
            _producers = producers;
            _logger = logger;
        }

        public Task SendToQueueAsync(CancellationToken cancellationToken)
        {
            var testMessage = new BagMessage("[{\"category\":[\"OrderCustomerOrderReceipt\"],\"email\":\"email@email.com\",\"event\":\"delivered\",\"ip\":\"169.89.79.142\",\"response\":\"250 Message Queued (F2BBED6F-AF8E-4532-B4B2-6490936E49C1.1)\",\"sg_event_id\":\"ZGVsaXZlcmVkLTAtMTk0MzQzMS1BWGJhaTlmV1FteXRGSUJ5QWdvWU5RLTA\",\"sg_message_id\":\"AXbai9fWQmytFIByAgoYNQ.filterdrecv-776cdd9c85-dt4n2-1-60E61EC4-3.0\",\"smtp-id\":\"<AXbai9fWQmytFIByAgoYNQ@geopod-ismtpd-2-2>\",\"timestamp\":1625693898,\"tls\":1}]\r\n");

            var messages = new List<BagMessage> { testMessage, testMessage };

            var queueOnly = _producers.First(x => x.Named == ServiceBusNames.QueuesOnly);

            return queueOnly.SendMessagesAsync(messages);
        }

        public Task SendToTopic(CancellationToken cancellationToken)
        {
            var uspsMessage = new DynamicMessage
            {
                OrderId = "1www",
                PrintDate = DateTimeOffset.Now,
                TrackingInfo = Guid.NewGuid().ToString()
            };

            var topic = _producers.First(x => x.Named == ServiceBusNames.TopicSubscriptions);

            // specify label/ subject
            return topic.SendMessageAsync(uspsMessage);
        }
    }
}
