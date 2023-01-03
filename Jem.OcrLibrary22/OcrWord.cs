using System.Text.Json.Serialization;

namespace Jem.OcrLibrary22;

public sealed class OcrWord : OcrElement
{
    #region ctor

    public OcrWord()
    {
    }

    //public OcrWord(CRect rect, CSize size) : base(rect, size)
    //{
    //}

    #endregion

    #region Symbols

    public int SymbolsCount { get => _Symbols.Count; }

    public IReadOnlyList<OcrSymbol> Symbols => _Symbols; private List<OcrSymbol> _Symbols = new();

    public OcrSymbol AddSymbol(OcrSymbol  oSymbol) 
    {
        _Symbols.Add(oSymbol);
        return oSymbol;
    }

    public override void Compile()
    {
        var first = true;
        short index = 0;
        foreach (var oSymbol in _Symbols)
        {
            oSymbol.Compile();

            oSymbol.Word = this;
            oSymbol.SymbolIndex = index;

            if (first)
            {
                this.TextDirection = oSymbol.TextDirection;
                this.Rect = oSymbol.Rect;
                first = false;
            }
            else
            {
                this.Rect = this.Rect.Union(oSymbol.Rect);
            }
        }
    }

    #endregion

    #region ToText

    public override void ToText(StringBuilder sb)
    {
        foreach (var symbol in Symbols)
        {
            sb.Append(symbol.Text);
        }
    }

    #endregion

    #region Binary

    internal void ReadFromBinary(BinaryReader br, byte version)
    {
        base.ReadBinary(br);

        this.WordIndex = br.ReadInt16();

        var symbolsCount = br.ReadInt32();
        for (int symbolIndex = 1; symbolIndex <= symbolsCount; symbolIndex++)
        {
            var symbol = new OcrSymbol();
            symbol.ReadFromBinary(br, version);
            AddSymbol(symbol);
        }
    }

    internal void WriteToBinary(BinaryWriter bw, byte version)
    {
        base.WriteBinary(bw);

        bw.Write((Int16)WordIndex);

        bw.Write((Int32)Symbols.Count);
        foreach (var symbol in Symbols)
        {
            symbol.WriteToBinary(bw, version);
        }
    }

    #endregion

    #region Owner

    [JsonIgnore] public OcrLine? Line { get; internal set; }
    public Int16 WordIndex { get; internal set; }

    #endregion

    #region Symbols

    public CRect Bounds
    {
        get
        {
            var bounds = new CRect();
            foreach (var symbol in Symbols)
            {
                bounds = bounds.Union(symbol.Rect);
            }
            return bounds;
        }
    }

    public float SingleSpaceWidth { get; set; }

    public IEnumerable<OcrSymbol> FindSymbols(string text, CRect bounds)
    {
        foreach (var symbol in Symbols)
        {
            if (!text.Contains(symbol.Text)) continue;
            if (!symbol.Rect.Inside(bounds)) continue;

            yield return symbol;
        }
    }

    #endregion

    public override void ApplyMoveScaleRotate(CTheory theory)
    {
        base.ApplyMoveScaleRotate(theory);
        foreach (var symbol in Symbols)
        {
            symbol.ApplyMoveScaleRotate(theory);
        }
    }

    public void RemoveSymbol(OcrSymbol oSymbol)
    {
        if (_Symbols.Contains(oSymbol))
            _Symbols.Remove(oSymbol);
    }

    //public void Clean()
    //{
    //}
}
