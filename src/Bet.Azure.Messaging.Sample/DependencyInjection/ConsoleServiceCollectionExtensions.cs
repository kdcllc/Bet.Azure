using Bet.Azure.Messaging.Options;
using Bet.Azure.Messaging.Sample;
using Bet.Azure.Messaging.Sample.EventHandlers;
using Bet.Azure.Messaging.Sample.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConsoleServiceCollectionExtensions
{
    public static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
    {
        services.AddScoped<IMain, Main>();

        var configuration = hostBuilder.Configuration;

        services.AddAzureServiceBus(
            ServiceBusNames.QueuesOnly,
            builder =>
            {
                var bagOptions = new QueueOptions();
                configuration.Bind($"AzureServiceBus:{ServiceBusNames.QueuesOnly}", bagOptions);

                builder.AddQueue<BagMessage>(bagOptions.Name);
                builder.AddConsumerHandler<BagMessage, BagMessageHandler>(bagOptions.Name);

                // automatically register consumers to be listening to the service bus.
                builder.RegisterConsumers();
            });

        services.AddAzureServiceBus(
            ServiceBusNames.TopicSubscriptions,
            builder =>
            {
                var dynOptions = new TopicOptions();
                configuration.Bind($"AzureServiceBus:{ServiceBusNames.TopicSubscriptions}", dynOptions);

                builder.AddTopic<DynamicMessage>(dynOptions.Name, dynOptions.SubscriptionName);

                // betbus, webhook-topic, create-order
                builder.AddConsumerHandler<DynamicMessageHandler>(dynOptions.MessageName, dynOptions.Name, dynOptions.SubscriptionName);

                // automatically register consumers to be listening to the service bus.
                builder.RegisterConsumers();
            });

        services.AddScoped<ProducerService>();
        services.AddScoped<ConsumerService>();
    }
}
