using System.Text.Json.Serialization;
using Bet.BuildingBlocks.Messaging.Abstractions;

namespace Bet.Azure.UnitTest.Messages;

public record TypedEventMessage : EventMessage
{

    /// <summary>
    /// The data of the paylod.
    /// </summary>
    [JsonInclude]
    public string Data { get; private set; }

    [JsonConstructor]
    public TypedEventMessage(string data)
    {
        Data = data;
    }

}
