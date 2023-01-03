using System.Runtime.Serialization;

namespace Jem.Profiling22.Data;

[Serializable]
public class FailedProfilingException : Exception
{
    public FailedProfilingException()
    {
    }

    public FailedProfilingException(string? message) : base(message)
    {
    }

    public FailedProfilingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected FailedProfilingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}