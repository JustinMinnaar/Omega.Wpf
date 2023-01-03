namespace Jem.CommonLibrary22;

[Serializable]
public class InvalidIdentifierException : ApplicationException
{
    public InvalidIdentifierException()
    {
    }

    public InvalidIdentifierException(string message) : base(message)
    {
    }

    protected InvalidIdentifierException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidIdentifierException(string message, Exception innerException) : base(message, innerException)
    {
    }
}