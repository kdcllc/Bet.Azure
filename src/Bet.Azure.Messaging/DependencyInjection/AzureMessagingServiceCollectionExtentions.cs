using System;

using Bet.Azure.Messaging;
using Bet.Azure.Messaging.Options;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureMessagingServiceCollectionExtentions
    {
        /// <summary>
        /// Adds Azure Service Bus with builder.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceBusName"></param>
        /// <param name="builder"></param>
        /// <param name="sectionName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddAzureServiceBus(
            this IServiceCollection services,
            string serviceBusName,
            Action<AzureMessagingServiceBuilder> builder,
            string sectionName = nameof(AzureServiceBusOptions),
            Action<AzureServiceBusOptions, IServiceProvider>? configure = default)
        {
            var @namespace = $"{serviceBusName}.servicebus.windows.net";

            services.AddChangeTokenOptions<AzureServiceBusOptions>(
                sectionName: sectionName,
                optionName: serviceBusName,
                configureAction: (opt, sp) =>
                {
                    configure?.Invoke(opt, sp);
                    opt.FullyQualifiedNamespace = @namespace;
                });

            services.TryAddSingleton<IAzureServiceBusConnection, AzureServiceBusConnection>();

            var poolBuilder = new AzureMessagingServiceBuilder(serviceBusName, services);
            builder(poolBuilder);

            services.AddSingleton<IAzureProducerPool>(sp =>
            {
                var connection = sp.GetRequiredService<IAzureServiceBusConnection>();
                var logger = sp.GetRequiredService<ILogger<AzureProducerPool>>();

                return new AzureProducerPool(connection, poolBuilder, logger);
            });

            services.AddSingleton<IAzureConsumerPool>(sp =>
            {
                var connection = sp.GetRequiredService<IAzureServiceBusConnection>();
                var logger = sp.GetRequiredService<ILogger<AzureConsumerPool>>();

                return new AzureConsumerPool(sp, connection, poolBuilder, logger);
            });

            return services;
        }
    }
}
