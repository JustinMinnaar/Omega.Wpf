namespace Jem.Profiling22.Data;

public class ProStamp : IdNamed
{
    public ProStamp()
    {
    }

    public ProStamp(string name, CSize allowMovement) : base(name)
    {
        AllowMovement = allowMovement;
    }

    public List<OcrSymbol> Symbols { get; set; } = new();
    public CSize AllowMovement { get; }
    public CRect Bounds
    {
        get
        {
            var r = new CRect();
            foreach (var symbol in Symbols)
                r = r.Union(symbol.Rect);
            return r;
        }
    }

    public void AddSymbols(OcrPage sourcePage, CRect sourceRect)
    {
        var symbols = sourcePage.ExtractSymbols(sourceRect);
        AddSymbols(symbols!);
    }

    public void AddSymbols(IEnumerable<OcrSymbol> symbols)
    {
        foreach (var symbol in symbols)
        {
            var newSymbol = new OcrSymbol(symbol);
            Symbols.Add(newSymbol);
        }
    }

    public void Replace(string find, string with, bool isMask)
    {
        foreach(var symbol in Symbols)
        {
            if (char.IsDigit(symbol.Text[0]))
                symbol.Text = "#";
        }
        // todo: implement
    }
}
