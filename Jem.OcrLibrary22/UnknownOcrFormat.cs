namespace Jem.OcrLibrary22;

[Serializable]
public class UnknownOcrFormat : Exception
{
    public UnknownOcrFormat()
    {
    }

    public UnknownOcrFormat(string? message) : base(message)
    {
    }

    public UnknownOcrFormat(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected UnknownOcrFormat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}