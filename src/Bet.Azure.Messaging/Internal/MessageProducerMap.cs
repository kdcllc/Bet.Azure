using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Bet.Azure.Messaging.Internal;

internal class MessageProducerMap
{
    private readonly ConcurrentDictionary<string, List<MessageProducerInfo>> _producerMappings = new();

    internal ReadOnlyDictionary<string, List<MessageProducerInfo>> ProducerMappings => new(_producerMappings);

    internal void AddQueueMapping(string namedClient, string queueOrTopicName, Type messageType)
    {
        Map(namedClient, queueOrTopicName, messageType, string.Empty);
    }

    internal void AddTopicMapping(string namedClient, string topicName, Type messageType, string subsciption)
    {
        Map(namedClient, topicName, messageType, subsciption);
    }

    private void Map(string namedClient, string queueOrTopicName, Type messageType, string subsciption)
    {
        // adds initial entry.
        if (!_producerMappings.ContainsKey(namedClient))
        {
            _producerMappings.TryAdd(namedClient, new());
        }

        if (_producerMappings[namedClient].Any(x => x.MessageType == messageType))
        {
            throw new ArgumentException($"Named Client {namedClient} already registered for '{nameof(messageType)}'", nameof(messageType));
        }

        _producerMappings[namedClient].Add(new MessageProducerInfo(queueOrTopicName, messageType, subsciption));
    }
}
