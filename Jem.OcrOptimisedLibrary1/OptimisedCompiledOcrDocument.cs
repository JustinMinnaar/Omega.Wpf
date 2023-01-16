using Jem.OcrLibrary22;

namespace Jem.OcrOptimisedLibrary1
{
    public sealed class OptimisedCompiledOcrDocument
    {
        public OptimisedCompiledOcrDocument(OcrDocument oDocument)
        {
            var list = new List<CompiledOcrPage>();
            foreach (var oPage in oDocument.Pages)
            {
                var cPage = new CompiledOcrPage(oPage);
                list.Add(cPage);
            }
            this.Pages = list.ToArray();
        }

        public CompiledOcrPage[] Pages { get; }
    }
}