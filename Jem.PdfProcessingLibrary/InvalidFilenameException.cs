using System.Runtime.Serialization;

namespace Jem.PdfProcessingLibrary
{
    [Serializable]
    internal class InvalidFilenameException : Exception
    {
        public InvalidFilenameException()
        {
        }

        public InvalidFilenameException(string? message) : base(message)
        {
        }

        public InvalidFilenameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidFilenameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}