namespace Jem.OcrLibrary22;

[Serializable]
public class OcrException : Exception
{
    public OcrException()
    {
    }

    public OcrException(string? message) : base(message)
    {
    }

    public OcrException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected OcrException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}