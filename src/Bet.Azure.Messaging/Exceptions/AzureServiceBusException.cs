namespace Bet.Azure.Messaging.Exceptions;

public class AzureServiceBusException : Exception
{
    public AzureServiceBusException() : base()
    {
    }

    public AzureServiceBusException(string message) : base(message)
    {
    }

    public AzureServiceBusException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
