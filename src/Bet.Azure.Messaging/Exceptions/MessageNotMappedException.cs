namespace Bet.Azure.Messaging.Exceptions;

public class MessageNotMappedException : Exception
{
    public MessageNotMappedException(string message) : base(message)
    {
    }

    public MessageNotMappedException() : base()
    {
    }

    public MessageNotMappedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
