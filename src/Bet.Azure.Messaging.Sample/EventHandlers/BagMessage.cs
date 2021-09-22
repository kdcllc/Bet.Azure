using System.Text.Json.Serialization;

using Bet.BuildingBlocks.Messaging.Abstractions;

namespace Bet.Azure.Messaging.Sample.EventHandlers
{
    public record BagMessage : EventMessage
    {

        /// <summary>
        /// The data of the paylod.
        /// </summary>
        [JsonInclude]
        public string Data { get; private set; }

        [JsonConstructor]

        public BagMessage(string data)
        {
            Data = data;
        }

    }
}
