namespace Jem.ProfilingLibrary23;

[Serializable]
internal class RowException : Exception
{
    public RowException()
    {
    }

    public RowException(string? message) : base(message)
    {
    }

    public RowException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected RowException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
