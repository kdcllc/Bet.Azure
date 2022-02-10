namespace Bet.Azure.Messaging.Internal;

internal class MessageProducerInfo
{
    public MessageProducerInfo(
        string queueOrTopicName,
        Type messageType,
        string subscription)
    {
        QueueOrTopicName = queueOrTopicName;
        MessageType = messageType;
        Subscription = subscription;
    }

    public string QueueOrTopicName { get; }

    public Type MessageType { get; }

    public string Subscription { get; }
}
