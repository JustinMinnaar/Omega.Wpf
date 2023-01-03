using Jem.CommonLibrary22;

using System.Text.Json.Serialization;

namespace Jem.OcrLibrary22;

public sealed class OcrLine : OcrElement
{
    private object padlock = new();

    public void Clean()
    {
        var lastleft = Rect.Left;

        var words = Words.ToList();
        OcrWord? lastWord = null;
        foreach (var oWord in words)
        {
            if (lastWord?.Symbols.Count > 0)
            {
                var lastSymbol = lastWord.Symbols[lastWord.Symbols.Count - 1];
                var right = lastSymbol.Rect.Right;
                var left = oWord.Rect.Left;
                var distance = Math.Abs(right - left);
                if (distance < 1)
                {
                    foreach (var oSymbol in oWord.Symbols.ToArray())
                    {
                        if (oSymbol.FontIndex != lastSymbol.FontIndex) continue;
                        if (oSymbol.FontSize != lastSymbol.FontSize) continue;
                        if (oSymbol.Color != lastSymbol.Color) continue;

                        lastWord.AddSymbol(oSymbol);
                        oWord.RemoveSymbol(oSymbol);
                    }
                }
            }

            lastWord = oWord;
        }
        //this.Text = ToText();
    }

    #region ctor

    public OcrLine()
    {
    }

    public OcrLine(string text, CRect rect, CSize size) : base(text, rect, size)
    {
    }

    #endregion

    #region Owner

    [JsonIgnore] public OcrBlock? Block { get; set; }
    public Int16 LineIndex { get; set; }

    #endregion

    #region Words

    public int WordsCount { get => _words.Count; }
    public IReadOnlyList<OcrWord> Words => _words;

    private List<OcrWord> _words = new();

    public void RemoveWord(OcrWord oWord)
    {
        _words.Remove(oWord);
    }

    public OcrWord AddWord(OcrWord oWord)
    {
        _words.Add(oWord);
        return oWord;
    }

    public override void Compile()
    {
        var first = true;
        Int16 index = 0;
        OcrWord? lastword = null;
        foreach (var oWord in _words.ToArray())
        {
            oWord.Compile();

            //if (lastword != null && Math.Abs(lastword.Bounds.Right - oWord.Bounds.Left) < 1 && Math.Abs(lastword.Bounds.Top - oWord.Bounds.Top) < 1)
            //{
            //    foreach (var symbol in oWord.Symbols.ToArray())
            //    {
            //        lastword.AddSymbol(symbol);
            //        // oWord.removesymbol(symbol);
            //    }
            //    RemoveWord(oWord);
            //    lastword.Rect = lastword.Bounds;
            //    continue;
            //}

            if (oWord.Symbols.Count == 0)
            {
                _words.Remove(oWord);
            }
            else lastword = oWord;

            oWord.Line = this;
            oWord.WordIndex = index;

            if (first)
            {
                this.TextDirection = oWord.TextDirection;
                this.Rect = oWord.Rect;
                first = false;
            }
            else
            {
                this.Rect = this.Rect.Union(oWord.Rect);
            }
        }
    }

    #endregion

    #region ToText

    public override void ToText(StringBuilder sb)
    {
        var first = true;
        foreach (var word in Words)
        {
            if (first)
                first = false;
            else
                sb.Append(' ');
            word.ToText(sb);
        }
    }

    #endregion

    #region Binary

    internal void WriteToBinary(BinaryWriter bw, byte version)
    {
        base.WriteBinary(bw);

        bw.Write((Int16)LineIndex);

        bw.Write((Int32)Words.Count);
        foreach (var word in Words)
        {
            word.WriteToBinary(bw, version);
        }
    }

    internal void ReadFromBinary(BinaryReader br, byte version)
    {
        base.ReadBinary(br);

        this.LineIndex = br.ReadInt16();

        var wordCount = br.ReadInt32();
        for (int i = 0; i < wordCount; i++)
        {
            var word = new OcrWord();
            word.ReadFromBinary(br, version);
            AddWord(word);
        }
    }

    #endregion

    #region Symbols

    public CRect Bounds
    {
        get
        {
            var bounds = new CRect();
            foreach (var word in Words)
            {
                bounds = bounds.Union(word.Bounds);
            }
            return bounds;
        }
    }

    public int SymbolsCount
    {
        get
        {
            var count = 0;
            foreach (var word in Words)
            {
                count += word.Symbols.Count;
            }
            return count;
        }
    }

    public IEnumerable<OcrSymbol> Symbols
    {
        get
        {
            foreach (var word in Words)
            {
                var symbols = word.Symbols;
                foreach (var symbol in symbols)
                    yield return symbol;
            }
        }
    }


    public IEnumerable<OcrSymbol> FindSymbols(string text, CRect bounds)
    {
        foreach (var word in Words)
        {
            var symbols = word.FindSymbols(text, bounds);
            foreach (var symbol in symbols)
                yield return symbol;
        }
    }

    #endregion

    public override void ApplyMoveScaleRotate(CTheory theory)
    {
        base.ApplyMoveScaleRotate(theory);
        foreach (var word in Words)
        {
            word.ApplyMoveScaleRotate(theory);
        }
    }

    public void RemoveSymbol(OcrSymbol oSymbol)
    {
        foreach (var oWord in Words)
        {
            oWord.RemoveSymbol(oSymbol);
            oWord.Rect = oWord.Bounds;
        }
    }

    public OcrPhrase? Extract(CRect rect, float maxSymbolHeight = OcrPage.DEFAULT_MAX_SYMBOL_HEIGHT, string? validCharacters = null)
    {
        var symbols = new List<OcrSymbol>();
        var sb = new StringBuilder();

        foreach (var word in this.Words)
        {
            if (!word.Rect.IntersectsWith(rect)) continue;

            bool beginWord = true;
            foreach (var symbol in word.Symbols)
            {
                if (!symbol.Rect.IntersectsWith(rect)) continue;

                if (symbol.Rect.Height > maxSymbolHeight) continue;

                if (beginWord && sb.Length > 0 && sb[sb.Length - 1] != ' ')
                    if (validCharacters == null || validCharacters.Contains(' '))
                        sb.Append(' ');
                beginWord = false;

                if (validCharacters == null || validCharacters.Contains(symbol.Text))
                {
                    sb.Append(symbol.Text);
                    symbols.Add(symbol);
                }
            }
        }

        if (symbols.Count > 0)
            return new(symbols, sb.ToString());
        else
            return null;
    }

    public static OcrLine Construct(string text, CPoint start, CSize symbolSize)
    {
        var line = new OcrLine();

        OcrWord? word = null;
        foreach (var character in text)
        {
            if (character == ' ')
            {
                word = null;
            }
            else
            {
                if (word == null) { word = new OcrWord(); line.AddWord(word); }

                var symbol = new OcrSymbol(new string(character, 1), new CRect(start, symbolSize));
                word.AddSymbol(symbol);
            }

            start.X += symbolSize.Width;
        }

        return line;
    }

    //public void AddTestSymbols(string line, CRect rect, double rotation)
    //{
    //    var t = new CTheory { Rotation = rotation, Move = new CSize(0, rect.Height) };

    //    var lines = text.Split('\n');
    //    foreach (var line in lines)
    //    {
    //        var index = this.Lines.Count;
    //        var oLine = new JOcrLine(this, index);
    //        this.Lines.Add(oLine);

    //        oLine.AddTestSymbols(line, rect, rotation);

    //        var p = t.AppliedMoveScaleRotate(rect.TopLeft);
    //        rect.TopLeft = p;
    //    }

    //}
}
