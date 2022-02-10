using Bet.BuildingBlocks.Messaging.Abstractions;
using Bet.BuildingBlocks.Messaging.Abstractions.Consumer;

namespace Microsoft.Extensions.DependencyInjection;

public interface IAzureMessagingServiceBuilder
{
    void AddConsumerHandler<THandler>(string messageName, string queueName) where THandler : class, IDynamicMessageConsumerHandler;

    void AddConsumerHandler<THandler>(string messageName, string topicName, string subscription) where THandler : class, IDynamicMessageConsumerHandler;

    void AddConsumerHandler<TModel, THandler>(string queueName)
        where TModel : EventMessage
        where THandler : class, IMessageConsumerHandler<TModel>;

    void AddConsumerHandler<TModel, THandler>(string queueName, string subscription)
        where TModel : EventMessage
        where THandler : class, IMessageConsumerHandler<TModel>;

    void AddQueue<T>(string queueName);

    void AddTopic<T>(string topicName, string subscription = "");

    void RegisterConsumers();
}
