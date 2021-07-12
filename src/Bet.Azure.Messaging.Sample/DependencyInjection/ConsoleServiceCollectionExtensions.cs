using Bet.Azure.Messaging.Sample;
using Bet.Azure.Messaging.Sample.EventMessages;
using Bet.Azure.Messaging.Sample.Handlers;
using Bet.Azure.Messaging.Sample.Services;

using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConsoleServiceCollectionExtensions
    {
        public static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
        {
            services.AddScoped<IMain, Main>();

            services.AddAzureServiceBus(
                ServiceBusNames.QueuesOnly,
                builder =>
                {
                    builder.AddQueue<SendGridEventMessage>("sendgridwebhook_dev");
                    builder.AddConsumerHandler<SendGridEventMessage, SendGridHandler>("sendgridwebhook_dev");
                });

            services.AddAzureServiceBus(
                ServiceBusNames.TopicSubscriptions,
                builder =>
                {
                    builder.AddTopic<Usps>("shipped", "test");
                    builder.AddConsumerHandler<UspsHandler>("Usps", "shipped", "test");
                });

            services.AddScoped<ProducerService>();
            services.AddScoped<ConsumerService>();
        }
    }
}
