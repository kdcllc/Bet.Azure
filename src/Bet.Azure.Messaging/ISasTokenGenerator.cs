namespace Bet.Azure.Messaging;

/// <summary>
/// Sas Token generator.
/// </summary>
public interface ISasTokenGenerator
{
    string Create(string topicName, int days, CancellationToken cancellationToken);
}
