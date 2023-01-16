using Jem.CommonLibrary22;
using Jem.OcrLibrary22;

namespace Jem.OcrOptimisedLibrary1
{
    [Serializable]
    public sealed class CompiledOcrIndexedSymbolsByCharacter : Dictionary<char, IndexedSymbols>
    {        
        public IEnumerable<OcrSymbol> Inside(char matchingChar, CRect searchRect)
        {
            if (this.TryGetValue(matchingChar, out var chomp))
            {
                foreach (var symbol in chomp.Symbols)
                {
                    if (symbol.Rect.Inside(searchRect)) yield return symbol;
                }
            }
        }
   }

}