using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.ServiceBus;

using Bet.Azure.Messaging.Exceptions;
using Bet.Azure.Messaging.Internal;
using Bet.Azure.Messaging.Messages;
using Bet.BuildingBlocks.Messaging.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bet.Azure.Messaging
{
    /// <summary>
    /// Azure Service Bus Producer Pool for sending messages.
    /// </summary>
    public class AzureProducerPool : IAzureProducerPool
    {
        private readonly IAzureServiceBusConnection _connection;
        private readonly ILogger<AzureProducerPool> _logger;
        private readonly ReadOnlyDictionary<string, List<MessageProducerInfo>> _mappings;

        public AzureProducerPool(
            IAzureServiceBusConnection connection,
            AzureMessagingServiceBuilder serviceBuilder,
            ILogger<AzureProducerPool> logger)
        {
            _connection = connection;
            _logger = logger;
            _mappings = serviceBuilder.ProducerMappings;
            Named = serviceBuilder.Named;
        }

        public string Named { get; }

        public Task SendMessageAsync<T>(
            T message,
            string subject = "",
            CancellationToken cancellationToken = default)
        {
            var messages = new T[1] { message };

            return SendMessagesAsync<T>(messages, subject, cancellationToken);
        }

        public async Task SendMessagesAsync<T>(
            IList<T> messages,
            string subject = "",
            CancellationToken cancellationToken = default)
        {
            var messageTypes = messages.Select(m => m?.GetType()).Distinct();

            EnsureMessagesAreMapped(messageTypes);

            var queuedMessages = new Queue<ServiceBusMessage>();
            var producerInfo = _mappings[Named].Find(x => x.MessageType == typeof(T));

            foreach (var message in messages)
            {
                var queuedMessage = new JsonSerializedMessage<T>(message);

                if (!string.IsNullOrEmpty(producerInfo?.Subscription))
                {
                    queuedMessage.Subject = producerInfo.Subscription;
                }

                if (!string.IsNullOrEmpty(subject))
                {
                    queuedMessage.Subject = subject;
                }

                queuedMessages.Enqueue(queuedMessage);
            }

            var client = _connection.CreateClient(Named);

            var sender = client.CreateSender(producerInfo.QueueOrTopicName);

            // create a message batch that we can send
            // total number of messages to be sent to the Service Bus queue
            var messageCount = messages.Count;

            // while all messages are not sent to the Service Bus queue
            while (queuedMessages.Count > 0)
            {
                // start a new batch
                using var messageBatch = await sender.CreateMessageBatchAsync(cancellationToken);

                // add the first message to the batch
                if (messageBatch.TryAddMessage(queuedMessages.Peek()))
                {
                    // dequeue the message from the .NET queue once the message is added to the batch
                    queuedMessages.Dequeue();
                }
                else
                {
                    // if the first message can't fit, then it is too large for the batch
                    throw new Exception($"Message {messageCount - messages.Count} is too large and cannot be sent.");
                }

                // add as many messages as possible to the current batch
                while (queuedMessages.Count > 0 && messageBatch.TryAddMessage(queuedMessages.Peek()))
                {
                    // dequeue the message from the .NET queue as it has been added to the batch
                    queuedMessages.Dequeue();
                }

                // now, send the batch
                await sender.SendMessagesAsync(messageBatch, cancellationToken);

                // if there are any remaining messages in the .NET queue, the while loop repeats
            }
        }

        public Task SendMessageAsync(
            EventMessage @event,
            string subject = "",
            CancellationToken cancellationToken = default)
        {
            var messages = new EventMessage[1] { @event };
            return SendMessagesAsync<EventMessage>(messages, subject, cancellationToken);
        }

        public Task SendMessagesAsync(
            IList<EventMessage> events,
            string subject = "",
            CancellationToken cancellationToken = default)
        {
            return SendMessagesAsync<EventMessage>(events, subject, cancellationToken);
        }

        internal void EnsureMessagesAreMapped(IEnumerable<Type?> messageTypes)
        {
            // Check that all of the message types have been registered first
            foreach (var messageType in messageTypes)
            {
                if (messageType != null)
                {
                    if (!_mappings[Named].Any(x => x.MessageType == messageType))
                    {
                        throw new MessageNotMappedException($"There is no {nameof(ServiceBusClient)} registered for the message type {messageType}");
                    }
                }
                else
                {
                    throw new MessageNotMappedException($"There is no {nameof(ServiceBusClient)} registered for the message type {messageType}");
                }
            }
        }
    }
}
