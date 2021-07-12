using System.Text.Json.Serialization;

using Bet.BuildingBlocks.Messaging.Abstractions;

namespace Bet.Azure.Messaging.Sample.EventMessages
{
    public record SendGridEventMessage : EventMessage
    {
        [JsonInclude]

        public string Data { get; private set; }

        [JsonConstructor]

        public SendGridEventMessage(string data)
        {
            Data = data;
        }

    }
}
