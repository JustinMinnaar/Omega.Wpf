﻿namespace Jem.CommonLibrary22;

[Serializable]
internal class NotInitializedException : Exception
{
    public NotInitializedException()
    {
    }

    public NotInitializedException(string? message) : base(message)
    {
    }

    public NotInitializedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected NotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}