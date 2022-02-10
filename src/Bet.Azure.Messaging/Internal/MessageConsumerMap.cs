using System.Collections.Concurrent;
using System.Collections.ObjectModel;

using Bet.BuildingBlocks.Messaging.Abstractions;
using Bet.BuildingBlocks.Messaging.Abstractions.Consumer;

namespace Bet.Azure.Messaging.Internal;

internal class MessageConsumerMap
{
    private readonly MessageConsumerPool _consumerPool = new();

    private readonly ConcurrentDictionary<string, List<MessageConsumerInfo>> _consumerMappings = new();

    internal ReadOnlyDictionary<string, List<MessageConsumerInfo>> ConsumerMappings => new(_consumerMappings);

    internal IMessageConsumerPool ConsumerPool => _consumerPool;

    internal void AddQueueMapping<TModel, THandler>(string namedClient, string queueName)
        where TModel : EventMessage
        where THandler : IMessageConsumerHandler<TModel>
    {
        var messageName = typeof(TModel).Name;

        Map<THandler>(namedClient, queueName, messageName);

        _consumerPool.AddHandler<TModel, THandler>();
    }

    internal void AddTopicMapping<TModel, THandler>(string namedClient, string topicName, string subscription)
        where TModel : EventMessage
        where THandler : IMessageConsumerHandler<TModel>
    {
        var messageName = typeof(TModel).Name;

        Map<THandler>(namedClient, topicName, messageName, subscription);

        _consumerPool.AddHandler<TModel, THandler>();
    }

    internal void AddQueueMapping<THandler>(string namedClient, string queueOrTopicName, string messageName)
        where THandler : IDynamicMessageConsumerHandler
    {
        Map<THandler>(namedClient, queueOrTopicName, messageName);
        _consumerPool.AddDynamicHandler<THandler>(messageName);
    }

    internal void AddTopicMapping<THandler>(string namedClient, string queueOrTopicName, string messageName, string subscription)
        where THandler : IDynamicMessageConsumerHandler
    {
        Map<THandler>(namedClient, queueOrTopicName, messageName, subscription);
        _consumerPool.AddDynamicHandler<THandler>(messageName);
    }

    private void Map<THandler>(string namedClient, string queueOrTopicName, string messageName, string subscription = "")
    {
        // adds initial entry.
        if (!_consumerMappings.ContainsKey(namedClient))
        {
            _consumerMappings.TryAdd(namedClient, new());
        }

        if (_consumerMappings[namedClient].Any(x => x.MessageName == messageName && x.Handler == typeof(THandler)))
        {
            throw new ArgumentException($"Named Client {namedClient} already registered for '{messageName}' with Handler {typeof(THandler).Name}");
        }

        _consumerMappings[namedClient].Add(new MessageConsumerInfo(namedClient, messageName, queueOrTopicName, subscription, typeof(THandler)));
    }
}
