using System.Text.Json.Serialization;

namespace Bet.Azure.Messaging.Sample.EventHandlers;

public class DynamicMessage
{
    [JsonPropertyName("orderId")]
    public string? OrderId { get; set; }

    [JsonPropertyName("trackingInfo")]
    public string? TrackingInfo { get; set; }

    [JsonPropertyName("printDate")]
    public DateTimeOffset? PrintDate { get; set; }
}
