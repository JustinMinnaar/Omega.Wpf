namespace Jem.Profiling22.Data;

using System.Runtime.Serialization;

[Serializable]
public class TextMismatchDefiningProfile : Exception
{
    public TextMismatchDefiningProfile()
    {
    }

    public TextMismatchDefiningProfile(string? message) : base(message)
    {
    }

    public TextMismatchDefiningProfile(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected TextMismatchDefiningProfile(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}