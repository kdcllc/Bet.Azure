using System;

using Azure.Messaging.ServiceBus;

namespace Bet.Azure.Messaging.Options
{
    public class AzureServiceBusOptions
    {
        /// <summary>
        /// Connection String can be empty.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Configuration for the <see cref="ServiceBusClientOptions"/>.
        /// </summary>
        internal Action<ServiceBusClientOptions>? ConfigureServiceBusClientOptions { get; set; }

        /// <summary>
        /// "yournamespace.servicebus.windows.net".
        /// </summary>
        internal string FullyQualifiedNamespace { get; set; } = string.Empty;
    }
}
