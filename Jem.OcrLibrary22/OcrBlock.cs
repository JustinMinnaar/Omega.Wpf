using System.Numerics;
using System.Text.Json.Serialization;

namespace Jem.OcrLibrary22;

public sealed class OcrBlock : OcrElement
{
    private object padlock = new ();

    public static OcrBlock Construct(params OcrLine[] lines)
    {
        var block = new OcrBlock();
        foreach (var line in lines)
            block.AddLine(line);
        return block;
    }

    public void Clean()
    {
        foreach(var line in Lines)
            line.Clean();

        //Text = ToText();
        Rect = Bounds;
    }

    #region ctor

    public OcrBlock()
    {
    }

    //public OcrBlock(string text, CRect rect, CSize size) : base(text, rect, size)
    //{
    //}

    #endregion

    #region Owner

    [JsonIgnore] public OcrPage? Page { get; internal set; }
    public Int16 BlockIndex { get; internal set; }

    #endregion

    #region Lines

    public int LinesCount { get => _Lines.Count; }

    public IReadOnlyList<OcrLine> Lines => _Lines; private List<OcrLine> _Lines = new();

    public void RemoveLine(OcrLine oLine)
    {
        _Lines.Remove(oLine); 
    }

    public void ClearLines()
    {
         _Lines.Clear(); 
    }

    public void AddLines(IEnumerable<OcrLine> oLines) 
    {
                    _Lines.AddRange(oLines);
    }

    public OcrLine AddLine(OcrLine oLine) 
    {
        _Lines.Add(oLine);
        return oLine;
    }

    public override void Compile()
    {
        var first = true;
        Int16 index = 0;

        //// If this line is touching the next line, they should be joined into one line.
        //OcrLine? lastLine = null;

        foreach (var oLine in _Lines.ToArray())
        {
            oLine.Compile();

            //if (lastLine != null && Math.Abs(lastLine.Rect.Top - oLine.Rect.Top) < 1 && Math.Abs(lastLine.Rect.Right - oLine.Rect.Left) < 1)
            //{
            //    //var diffTop = (lastLine.Rect.Top - oLine.Rect.Top);
            //    foreach (var oWord in oLine.Words.ToArray())
            //    {
            //        lastLine.AddWord(oWord);
            //        oLine.RemoveWord(oWord);
            //    }
            //    lastLine.Rect = lastLine.Bounds;
            //    RemoveLine(oLine);
            //    continue;
            //}

            if (oLine.Words.Count == 0)
            {
                _Lines.Remove(oLine);
                continue;
            }
            //            else lastLine = oLine;

            oLine.Rect = oLine.Bounds;
            oLine.Block = this;
            oLine.LineIndex = index++;

            if (first)
            {
                this.TextDirection = oLine.TextDirection;
                this.Rect = oLine.Rect;
                first = false;
            }
            else
            {
                this.Rect = this.Rect.Union(oLine.Rect);
            }
        }
    }

    #endregion

    #region ToText

    public override void ToText(StringBuilder sb)
    {
        var first = true;
        foreach (var oLine in Lines)
        {
            if (first)
                first = false;
            else
                sb.Append(' ');

            oLine.ToText(sb);
        }
    }

    #endregion

    #region Binary

    internal void ReadFromBinary(BinaryReader br, byte version)
    {
        base.ReadBinary(br);

        this.BlockIndex = br.ReadInt16();

        var lineCount = br.ReadInt32();
        for (int lineIndex = 1; lineIndex <= lineCount; lineIndex++)
        {
            var oLine = new OcrLine();
            oLine.ReadFromBinary(br, version);
            AddLine(oLine);
        }
    }

    internal void WriteToBinary(BinaryWriter bw, byte version)
    {
        base.WriteBinary(bw);

        bw.Write((Int16)BlockIndex);

        bw.Write((Int32)Lines.Count);
        foreach (var oLine in Lines)
        {
            oLine.WriteToBinary(bw, version);
        }
    }

    #endregion

    #region Symbols

    public CRect Bounds
    {
        get
        {
            var bounds = new CRect();
            foreach (var oLine in Lines)
            {
                bounds = bounds.Union(oLine.Bounds);
            }
            return bounds;
        }
    }

    public int SymbolsCount
    {
        get
        {
            var count = 0;
            foreach (var oLine in Lines)
            {
                count += oLine.SymbolsCount;
            }
            return count;
        }
    }

    public IEnumerable<OcrSymbol> Symbols
    {
        get
        {
            foreach (var oLine in Lines)
            {
                var symbols = oLine.Symbols;
                foreach (var symbol in symbols)
                    yield return symbol;
            }
        }
    }


    public IEnumerable<OcrSymbol> FindSymbols(string text, CRect bounds)
    {
        foreach (var oLine in Lines)
        {
            var symbols = oLine.FindSymbols(text, bounds);
            foreach (var symbol in symbols)
                yield return symbol;
        }
    }

    #endregion

    public override void ApplyMoveScaleRotate(CTheory theory)
    {
        base.ApplyMoveScaleRotate(theory);
        foreach (var oLine in Lines)
        {
            oLine.ApplyMoveScaleRotate(theory);
        }
    }

    public void RemoveSymbol(OcrSymbol oSymbol)
    {
        foreach (var oLine in Lines)
        {
            oLine.RemoveSymbol(oSymbol);
            oLine.Rect = oLine.Bounds;
        }
    }

    //public void AddTestSymbols(string text, double x, double y, double rotation) =>
    //    AddTestSymbols(text, new CPoint(x, y), rotation);

    //public void AddTestSymbols(string text, CPoint point, double rotation) =>
    //    AddTestSymbols(text, new CRect(point, new CSize(text.Length * 10, 20)), rotation);

    //public void AddTestSymbols(string text, CRect rect, double rotation)
    //{
    //    var t = new CTheory { Rotation = rotation, Move = new CSize(0, rect.Height) };

    //    var lines = text.Split('\n');
    //    foreach (var oLine in lines)
    //    {
    //        var index = this.Lines.Count;
    //        var oLine = new JOcrLine(this, index);
    //        this.Lines.Add(oLine);

    //        oLine.AddTestSymbols(oLine, rect, rotation);

    //        var p = t.AppliedMoveScaleRotate(rect.TopLeft);
    //        rect.TopLeft = p;
    //    }
    //}

}
