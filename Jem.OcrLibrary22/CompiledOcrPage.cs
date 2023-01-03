using Jem.CommonLibrary22;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jem.OcrLibrary22;

public sealed class CompiledOcrPage
{
    public int PageIndex { get; }
    // public SkipKeyValue? MustSkip { get; } now only document
    public CRect PageRect { get; }

    public COcrIndexedSymbolsByCharacter AllSymbols { get; }

    public CompiledOcrPage(BinaryReader br)
    {
        PageRect = CRect.ReadFromBinaryFloat(br);
        PageIndex = br.ReadInt32();
        AllSymbols = new();
        AllSymbols.ReadBinary(br);
    }

    public CompiledOcrPage(OcrPage ocrPage)
    {
        PageRect = ocrPage.Rect;
        PageIndex = ocrPage.PageIndex;
        //MustSkip = ocrPage.MustSkip;
        var pageSymbols = (from s in ocrPage.Symbols
                           select new OcrSymbol(s)
                           ).ToList();
        AllSymbols = Build(pageSymbols);
    }

    private COcrIndexedSymbolsByCharacter Build(IEnumerable<OcrSymbol> symbols)
    {
        var builtSymbols = new COcrIndexedSymbolsByCharacter();
        foreach (var symbol in symbols)
        {
            var c = symbol.Text[0];
            if (builtSymbols.ContainsKey(c))
                builtSymbols[c].Add(symbol);
            else
                builtSymbols.Add(c, new IndexedSymbols { symbol });
        }
        return builtSymbols;
    }

    public void SaveBinaryFile(string ocrPath)
    {
        using var stream = File.OpenWrite(ocrPath);
        var bw = new BinaryWriter(stream);
        WriteBinary(bw);
    }

    public static CompiledOcrPage? TryLoadBinaryFile(string ocrPath)
    {
        if (!File.Exists(ocrPath)) return null;

        using var stream = File.OpenRead(ocrPath);
        var br = new BinaryReader(stream);
        var cPage = new CompiledOcrPage(br);
        return cPage;
    }

    public void WriteBinary(BinaryWriter bw)
    {
        PageRect.WriteToBinaryFloat(bw);
        bw.Write(PageIndex);
        AllSymbols.WriteBinary(bw);
    }



}

public sealed class COcrIndexedSymbolsByCharacter : Dictionary<char, IndexedSymbols>
{
    public void WriteBinary(BinaryWriter bw)
    {
        bw.Write(this.Count);
        foreach (var kv in this)
        {
            bw.Write(kv.Key);
            kv.Value.WriteBinary(bw);
        }
    }

    public void ReadBinary(BinaryReader br)
    {
        var count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var key = br.ReadChar();
            var value = new IndexedSymbols();
            value.ReadBinary(br);
            this.Add(key, value);
        }
    }

    public bool AnyInside(char matchingChar, CRect searchRect)
    {
        if (this.TryGetValue(matchingChar, out var symbols))
        {
            foreach (var symbol in symbols)
            {
                if (symbol.Rect.Inside(searchRect)) return true;
            }
        }
        return false;
    }

    public int CountInside(char matchingChar, CRect searchRect)
    {
        var count = 0;
        if (this.TryGetValue(matchingChar, out var symbols))
        {
            foreach (var symbol in symbols)
            {
                if (symbol.Rect.Inside(searchRect)) count++;
            }
        }
        return count;
    }

    public IEnumerable<OcrSymbol> Inside(char matchingChar, CRect searchRect)
    {
        //matchingChar = char.ToLowerInvariant(matchingChar);
        if (this.TryGetValue(matchingChar, out var symbols))
        {
            foreach (var symbol in symbols)
            {
                if (symbol.Rect.Inside(searchRect)) yield return symbol;
            }
        }
    }

    public bool AnyTouches(char matchingChar, CRect searchRect)
    {
        if (this.TryGetValue(matchingChar, out var symbols))
        {
            foreach (var symbol in symbols)
            {
                if (symbol.Rect.IntersectsWith(searchRect)) return true;
            }
        }
        return false;
    }

    public int CountTouches(char matchingChar, CRect searchRect)
    {
        var count = 0;
        if (this.TryGetValue(matchingChar, out var symbols))
        {
            foreach (var symbol in symbols)
            {
                if (symbol.Rect.IntersectsWith(searchRect)) count++;
            }
        }
        return count;
    }

    public IEnumerable<OcrSymbol> Touches(char matchingChar, CRect searchRect)
    {
        //matchingChar = char.ToLowerInvariant(matchingChar);
        if (this.TryGetValue(matchingChar, out var symbols))
        {
            foreach (var symbol in symbols)
            {
                if (symbol.Rect.IntersectsWith(searchRect)) yield return symbol;
            }
        }
    }
}

public sealed class IndexedSymbols : List<OcrSymbol>
{
    public IEnumerable<OcrSymbol> Inside(char matchingChar, CRect searchRect)
    {
        foreach (var symbol in this)
        {
            if (symbol.Rect.Inside(searchRect)) yield return symbol;
        }
    }

    public void ReadBinary(BinaryReader br)
    {
        var count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var symbol = new OcrSymbol();
            symbol.ReadBinary(br);
            this.Add(symbol);
        }
    }

    public void WriteBinary(BinaryWriter bw)
    {
        bw.Write(this.Count);
        foreach (var symbol in this)
        {
            symbol.WriteBinary(bw);
        }
    }
}