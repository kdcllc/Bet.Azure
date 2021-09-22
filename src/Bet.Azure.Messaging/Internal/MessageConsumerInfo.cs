using System;

namespace Bet.Azure.Messaging.Internal
{
    internal class MessageConsumerInfo
    {
        public MessageConsumerInfo(
            string named,
            string messageName,
            string queueOrTopicName,
            string subscription,
            Type handler)
        {
            Named = named;
            MessageName = messageName;
            QueueOrTopicName = queueOrTopicName;
            Subscription = subscription;
            Handler = handler;
        }

        public string Named { get; }

        public string MessageName { get; }

        public string QueueOrTopicName { get; }

        public string Subscription { get; }

        public Type Handler { get; }
    }
}
