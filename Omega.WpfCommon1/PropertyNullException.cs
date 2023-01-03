using System;
using System.Runtime.Serialization;

namespace Omega.WpfCommon1;

[Serializable]
public class PropertyNullException : Exception
{
    public PropertyNullException()
    {
    }

    public PropertyNullException(string? message) : base(message)
    {
    }

    public PropertyNullException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected PropertyNullException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}