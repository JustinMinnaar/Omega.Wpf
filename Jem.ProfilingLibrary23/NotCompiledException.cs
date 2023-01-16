using System.Runtime.Serialization;

namespace Jem.ProfilingLibrary23
{
    [Serializable]
    internal class NotCompiledException : Exception
    {
        public NotCompiledException()
        {
        }

        public NotCompiledException(string? message) : base(message)
        {
        }

        public NotCompiledException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NotCompiledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}