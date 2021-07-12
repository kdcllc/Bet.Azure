using System.Collections.Generic;
using System.Collections.ObjectModel;

using Bet.Azure.Messaging.Internal;
using Bet.BuildingBlocks.Messaging.Abstractions;
using Bet.BuildingBlocks.Messaging.Abstractions.Consumer;

namespace Microsoft.Extensions.DependencyInjection
{
    public sealed class AzureMessagingServiceBuilder
    {
        private readonly MessageProducerMap _producers = new ();
        private readonly MessageConsumerMap _consumers = new ();

        public AzureMessagingServiceBuilder(string named, IServiceCollection services)
        {
            Named = named;
            Services = services;
        }

        internal string Named { get; }

        internal IServiceCollection Services { get; }

        internal ReadOnlyDictionary<string, List<MessageProducerInfo>> ProducerMappings => _producers.ProducerMappings;

        internal IMessageConsumerPool ConsumerPool => _consumers.ConsumerPool;

        internal ReadOnlyDictionary<string, List<MessageConsumerInfo>> ConsumerMappings => _consumers.ConsumerMappings;

        public void AddQueue<T>(string queueName)
        {
            _producers.AddQueueMapping(Named, queueName, typeof(T));
        }

        public void AddTopic<T>(string topicName, string subscription = "")
        {
            _producers.AddTopicMapping(Named, topicName, typeof(T), string.IsNullOrEmpty(subscription) ? typeof(T).Name : subscription);
        }

        public void AddConsumerHandler<TModel, THandler>(string queueName)
            where TModel : EventMessage
            where THandler : class, IMessageConsumerHandler<TModel>
        {
            _consumers.AddQueueMapping<TModel, THandler>(Named, queueName);
            Services.AddTransient<THandler>();
        }

        public void AddConsumerHandler<TModel, THandler>(string queueName, string subscription)
           where TModel : EventMessage
           where THandler : class, IMessageConsumerHandler<TModel>
        {
            _consumers.AddTopicMapping<TModel, THandler>(Named, queueName, subscription);
            Services.AddTransient<THandler>();
        }

        public void AddConsumerHandler<THandler>(string messageName, string queueName)
            where THandler : class, IDynamicMessageConsumerHandler
        {
            _consumers.AddQueueMapping<THandler>(Named, queueName, messageName);
            Services.AddTransient<THandler>();
        }

        public void AddConsumerHandler<THandler>(string messageName, string topicName, string subscription)
           where THandler : class, IDynamicMessageConsumerHandler
        {
            _consumers.AddTopicMapping<THandler>(Named, topicName, messageName, subscription);
            Services.AddTransient<THandler>();
        }
    }
}
