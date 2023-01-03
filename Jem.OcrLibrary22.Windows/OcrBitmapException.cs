using System.Runtime.Serialization;

namespace Jem.OcrLibrary22.Windows;

[Serializable]
internal class OcrBitmapException : Exception
{
    public OcrBitmapException()
    {
    }

    public OcrBitmapException(string? message) : base(message)
    {
    }

    public OcrBitmapException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected OcrBitmapException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}