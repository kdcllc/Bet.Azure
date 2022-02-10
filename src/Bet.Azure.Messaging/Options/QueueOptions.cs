namespace Bet.Azure.Messaging.Options;

/// <summary>
/// The options for the consumer of the messages.
/// </summary>
public class QueueOptions
{
    /// <summary>
    /// Name of the queue.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
