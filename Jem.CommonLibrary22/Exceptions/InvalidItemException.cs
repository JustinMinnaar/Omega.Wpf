namespace Jem.CommonLibrary22;

[Serializable]
public class InvalidItemException : ApplicationException
{
    public InvalidItemException()
    {
    }

    public InvalidItemException(string message) : base(message)
    {
    }

    protected InvalidItemException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidItemException(string message, Exception innerException) : base(message, innerException)
    {
    }
}