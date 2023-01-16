using System.Runtime.Serialization;

namespace Jem.PdfProcessingLibrary
{
    [Serializable]
    internal class JemPdfException : Exception
    {
        public JemPdfException()
        {
        }

        public JemPdfException(string? message) : base(message)
        {
        }

        public JemPdfException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected JemPdfException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}