using Bet.BuildingBlocks.Messaging.Abstractions.Producer;

namespace Bet.Azure.Messaging;

public interface IAzureProducerPool : IMessageProducer
{
    /// <summary>
    /// Utilizes named string parameter to discover the producer.
    /// </summary>
    string Named { get; }
}
