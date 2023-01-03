using System.Runtime.Serialization;

namespace Jem.Profiling22.Data;

[Serializable]
internal class EmptySkipableException : Exception
{
    public EmptySkipableException()
    {
    }

    public EmptySkipableException(string? message) : base(message)
    {
    }

    public EmptySkipableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected EmptySkipableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}