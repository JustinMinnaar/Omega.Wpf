using Jem.OcrLibrary22;

namespace Jem.ProfilingLibrary23
{
    public record JValues
    {
        public Dictionary<string, OcrLine> Values { get; set; } = new();
    }
}