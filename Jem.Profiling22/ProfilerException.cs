using System.Runtime.Serialization;

namespace Jem.Profiling22;

[Serializable]
public class ProfilerException : Exception
{
    public ProfilerException()
    {
    }

    public ProfilerException(string? message) : base(message)
    {
    }

    public ProfilerException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ProfilerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}