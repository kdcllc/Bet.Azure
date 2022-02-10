using System.Text.Json;

using Azure.Messaging.ServiceBus;

using Bet.BuildingBlocks.Messaging.Abstractions;

namespace Bet.Azure.Messaging.Messages;

public class JsonSerializedMessage<T> : ServiceBusMessage
{
    public JsonSerializedMessage(T messageContent) : base(JsonSerializer.SerializeToUtf8Bytes(messageContent))
    {
        ContentType = "application/json";
        MessageId = Guid.NewGuid().ToString();
        SessionId = Guid.NewGuid().ToString();
        Subject = typeof(T).Name.Replace(nameof(EventMessage), string.Empty);
        ApplicationProperties.Add("messageName", typeof(T).Name);
    }
}
