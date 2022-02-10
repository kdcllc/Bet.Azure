namespace Bet.Azure.Messaging.Options;

/// <summary>
/// The options for the consumer of the messages.
/// </summary>
public class TopicOptions : QueueOptions
{
    /// <summary>
    /// The name of the message, corresponds to .NET object type of the message.
    /// </summary>
    public string MessageName { get; set; } = string.Empty;

    /// <summary>
    /// The name of the subsciption is present.
    /// </summary>
    public string SubscriptionName { get; set; } = string.Empty;
}
