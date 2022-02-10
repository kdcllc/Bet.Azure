namespace Bet.Azure.Messaging.Options;

public class SasTokenGeneratorOptions
{
    /// <summary>
    /// SAS Key from the vault.
    /// </summary>
    public string? SasKey { get; set; }

    /// <summary>
    /// The name of the SasKey i.e. prodAccessKey.
    /// </summary>
    public string? SasKeyName { get; set; }

    /// <summary>
    /// Azure Service Bus Namespace.
    /// </summary>
    public string? Namespace { get; set; }
}
