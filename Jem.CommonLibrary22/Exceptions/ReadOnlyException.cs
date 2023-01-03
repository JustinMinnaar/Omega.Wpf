namespace Jem.CommonLibrary22;

[Serializable]
public class ReadOnlyException : ApplicationException
{
    public ReadOnlyException()
    {
    }

    public ReadOnlyException(string message) : base(message)
    {
    }

    protected ReadOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ReadOnlyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}