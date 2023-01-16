using Jem.CommonLibrary22;
using Jem.OcrLibrary22;

namespace Jem.OcrOptimisedLibrary1;

public sealed class CompiledOcrPage
{
    public readonly CRect PageRect;

    public readonly CompiledOcrIndexedSymbolsByCharacter AllSymbolsByCharacter;

    public CompiledOcrPage(OcrPage ocrPage)
    {
        PageRect = ocrPage.Rect;
        AllSymbolsByCharacter = Build(ocrPage.Symbols);
    }

    private static CompiledOcrIndexedSymbolsByCharacter Build(IEnumerable<OcrSymbol> symbols)
    {
        var temp = new Dictionary<char, List<OcrSymbol>>();
        foreach (var symbol in symbols)
        {
            var c = symbol.Text[0];
            if (!temp.ContainsKey(c))
                temp.Add(c, new());
            temp[c].Add(symbol);
        }

        var symbolsByCharacter = new CompiledOcrIndexedSymbolsByCharacter();
        foreach (var kv in temp)
            symbolsByCharacter.Add(kv.Key, new IndexedSymbols(kv.Value));
        return symbolsByCharacter;
    }
}