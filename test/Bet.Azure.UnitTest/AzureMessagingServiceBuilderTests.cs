using Bet.Azure.Messaging;
using Bet.Azure.Messaging.Options;
using Bet.Azure.UnitTest.EventHandlers;
using Bet.Azure.UnitTest.Messages;
using Bet.Extensions.Testing.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bet.Azure.UnitTest;

public class AzureMessagingServiceBuilderTests
{
    private readonly ITestOutputHelper _output;

    public AzureMessagingServiceBuilderTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Register_Two_Messages_With_One_Queue_Producer_And_Consumer_Successfully()
    {
        var queueName1 = "myQueue1";
        var queueName2 = "myQueue2";

        var dict = new Dictionary<string, string?>
        {
            { "AzureServiceBus:Queue1:Name", queueName1 },
            { "AzureServiceBus:Queue2:Name", queueName2 }
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(dict).Build();

        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(configuration);

        services.AddLogging(x => x.AddXunit(_output));

        var serviceBusName = "testservicebus";

        services.AddAzureServiceBus(
            serviceBusName,
            builder =>
            {
                var bagOptions = new QueueOptions();
                configuration.Bind("AzureServiceBus:Queue1", bagOptions);

                // adds producer for queue 1
                builder.AddQueue<TypedEventMessage>(bagOptions.Name);

                // adds consumer of the same event
                builder.AddConsumerHandler<TypedEventMessage, TypedEventMessageHandler>(bagOptions.Name);

                // rebind configurations
                configuration.Bind("AzureServiceBus:Queue2", bagOptions);

                // adds producer for queue 2
                builder.AddQueue<DynamicMessage>(bagOptions.Name);

                // adds consumer of the same event
                builder.AddConsumerHandler<DynamicMessageHandler>("customerObject", bagOptions.Name);

                // automatically register consumers to be listening to the service bus.
                builder.RegisterConsumers();
            });

        var sp = services.BuildServiceProvider();

        // check produces first
        var producers = sp.GetServices<IAzureProducerPool>();

        // should contain only a single producer.
        Assert.Single(producers);

        Assert.Equal(serviceBusName, producers.First().Named);

        // check consumers
        var handler1 = sp.GetRequiredService<TypedEventMessageHandler>();
        Assert.NotNull(handler1);

        var handler2 = sp.GetRequiredService<DynamicMessageHandler>();
        Assert.NotNull(handler2);

        var hosts = sp.GetServices<IHostedService>();
        Assert.Single(hosts);
    }

    [Fact]
    public void Register_One_Message_With_One_Queue_Producer_And_Consumer_Successfully()
    {
        var queueName = "myQueue1";

        var dict = new Dictionary<string, string?>
        {
            { "AzureServiceBus:Queue:Name", queueName }
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(dict).Build();

        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(configuration);

        services.AddLogging(x => x.AddXunit(_output));

        var serviceBusName = "testservicebus";

        services.AddAzureServiceBus(
            serviceBusName,
            builder =>
            {
                var bagOptions = new QueueOptions();
                configuration.Bind("AzureServiceBus:Queue", bagOptions);

                // adds producer
                builder.AddQueue<TypedEventMessage>(bagOptions.Name);

                // adds consumer of the same event
                builder.AddConsumerHandler<TypedEventMessage, TypedEventMessageHandler>(bagOptions.Name);

                // automatically register consumers to be listening to the service bus.
                builder.RegisterConsumers();
            });

        var sp = services.BuildServiceProvider();

        // check produces first
        var producers = sp.GetServices<IAzureProducerPool>();

        // should contain only a single producer.
        Assert.Single(producers);

        Assert.Equal(serviceBusName, producers.First().Named);

        // check consumers
        var handler = sp.GetRequiredService<TypedEventMessageHandler>();
        Assert.NotNull(handler);

        var hosts = sp.GetServices<IHostedService>();
        Assert.Single(hosts);
    }
}
