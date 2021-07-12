using System;
using System.Text.Json.Serialization;

#nullable disable
namespace Bet.Azure.Messaging.Sample.EventMessages
{
    public class Usps
    {
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("trackingInfo")]
        public string TrackingInfo { get; set; }

        [JsonPropertyName("printDate")]
        public DateTime PrintDate { get; set; }
    }
}
