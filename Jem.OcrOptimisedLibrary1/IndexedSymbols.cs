using Jem.CommonLibrary22;
using Jem.OcrLibrary22;

namespace Jem.OcrOptimisedLibrary1
{
    [Serializable]
    public sealed class IndexedSymbols
    {
        public IndexedSymbols(IEnumerable<OcrSymbol> symbols)
        {
            Symbols = symbols.ToArray();
        }

        public OcrSymbol[] Symbols;

        public IEnumerable<OcrSymbol> Inside(CRect searchRect)
        {
            foreach (var symbol in this.Symbols)
            {
                if (symbol.Rect.Inside(searchRect)) yield return symbol;
            }
        }

        public IEnumerable<OcrSymbol> Inside(char matchingChar, CRect searchRect)
        {
            foreach (var symbol in this.Symbols)
            {
                if (symbol.Text[0] != matchingChar) continue;
                if (symbol.Rect.Inside(searchRect)) yield return symbol;
            }
        }
    }

}