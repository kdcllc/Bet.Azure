namespace Bet.Azure.Messaging.Sample;

/// <summary>
/// Namespaces names for azure service bus.
/// </summary>
public static class ServiceBusNames
{
    /// <summary>
    /// https://betqueue.servicebus.windows.net/.
    /// </summary>
    public const string QueuesOnly = "betqueue";

    /// <summary>
    /// https://betbus.servicebus.windows.net/.
    /// </summary>
    public const string TopicSubscriptions = "betbus";
}
