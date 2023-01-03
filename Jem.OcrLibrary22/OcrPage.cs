using System.ComponentModel.Design;
using System.Drawing;
using System.Text.Json.Serialization;

namespace Jem.OcrLibrary22;

public sealed class OcrPage : OcrElement
{
    private object padlock = new();

    public static OcrPage Construct(params OcrBlock[] blocks)
    {
        var page = new OcrPage();
        foreach (var block in blocks)
            page.AddBlock(block);
        return page;
    }

    public override string ToString()
    {
        return $"index:{PageIndex} symbols:{Symbols.Count()} rect:{Rect} bounds:{Bounds}";
    }

    public const float DEFAULT_MAX_SYMBOL_HEIGHT = 30f;

    public void Clean()
    {
        // MergeLinesThatTouch();

        //var sb = new StringBuilder();
        foreach (var block in Blocks)
        {
            block.Clean();
            Rect = Bounds;
            //sb.AppendLine(block.Text);
        }
        //Text = sb.ToString();

        //    currentBlockLines = currentBlockLines.OrderBy(l => l.Rect.Top).ToList();

        //    block.ClearLines();
        //    block.AddLines(currentBlockLines);

        //    //var blocks = new List<OcrBlock>();
        //    //OcrBlock? currentBlock = null;

        //    //var gap = 30;
        //    //var lineHeight = 0;
        //    //foreach (var line in currentBlockLines)
        //    //{

        //    //    if (currentBlock == null || line.Rect.Top > currentBlock.Rect.Bottom + gap || (int)(line.Rect.Height / 10) != (int)(lineHeight / 10))
        //    //    {
        //    //        currentBlock = new();
        //    //        blocks.Add(currentBlock);
        //    //    }
        //    //    currentBlock.AddLine(line);
        //    //    currentBlock.Rect = currentBlock.Bounds;
        //    //    lineHeight = (int)line.Rect.Height;
        //    //}

        //}

        //_Blocks = blocks;
        Rect = Bounds;
    }

    private void MergeLinesThatTouch()
    {

        //var l01 = this.Blocks[0].Lines[0];
        //var l11 = this.Blocks[1].Lines[0];

        //OcrLine? lastLine = null;
        //var lines = (from oBlock in this.Blocks
        //             from oLine in oBlock.Lines
        //             select oLine)
        //             .Where(a => a.Rect.Height <= 40)
        //             .OrderBy(a => a.Rect.Top)
        //             .ToList();

        //var blocks = new List<OcrBlock>();
        //OcrBlock? currentBlock = null;
        //var blockBorderSize = 20;

        //for (int i = 0; i < lines.Count - 1; i++)
        //{
        //    var block = new OcrBlock();
        //    blocks.Add(block);

        //    var thisLine = lines[i];
        //    block.AddLine(thisLine);

        //    for (int j = i + 1; j < lines.Count; j++)
        //    {
        //        var otherLine = lines[j];
        //        var thisRect = thisLine.Rect.Inflated(blockBorderSize, blockBorderSize);
        //        var otherRect = otherLine.Rect.Inflated(blockBorderSize, blockBorderSize);
        //        if (thisRect.IntersectsWith(otherRect))
        //        {
        //            block.AddLine(otherLine);
        //        }
        //    }
        //}

        //foreach (var thisLine in lines)
        //{

        //    // Get the first line to process
        //    if (lastLine == null) { lastLine = thisLine; }
        //    else
        //    {
        //        // The lines must be touching each other (within 2 pixels)
        //        var lastRect = lastLine.Rect;
        //        var thisRect = thisLine.Rect;
        //        var distance = Math.Abs(thisRect.Top - lastRect.Bottom);
        //        if (distance > 6) currentBlock = null;
        //    }

        //    // Add this line to the block and continue
        //    if (currentBlock == null) { currentBlock = new(); blocks.Add(currentBlock); }
        //    currentBlock.AddLine(thisLine);
        //}

        //_Blocks = blocks;
    }

    #region ctor

    public OcrPage()
    {
    }

    //public OcrPage(string text, CRect rect, CSize size) : base(text, rect, size)
    //{

    //}

    #endregion

    #region Owner

    [JsonIgnore] public OcrDocument? OwnerDocument { get; set; }

    public int PageIndex { get; set; }

    /// <summary>The number of the document could be letters, symbols, digits, etc. and is determined from the actual file.</summary>
    public string? Number { get; set; }

    #endregion

    #region ToText

    public override void ToText(StringBuilder sb)
    {
        foreach (var block in Blocks)
        {
            block.ToText(sb);
            sb.AppendLine(); // blank line between blocks
        }
    }

    //public string ToText() => ToText(0);

    //public string ToText(int maxLength)
    //{
    //    double x = -1;
    //    var sb = new StringBuilder();
    //    foreach (var symbol in Symbols)
    //    {
    //        if (x == -1) x = symbol.Rect.Left;
    //        else if (symbol.Rect.Left < x - 1) { sb.AppendLine(); x = symbol.Rect.Left; }
    //        else
    //        if (symbol.Rect.Left > x + 1) { sb.Append(' '); x = symbol.Rect.Left; }

    //        sb.Append(symbol.Character);
    //        x += symbol.Rect.Width;

    //        if (maxLength > 0 && sb.Length >= maxLength) break;
    //    }
    //    return sb.ToString();
    //}

    #endregion

    #region Properties

    public int CountImagesClassifiedAsOcr { get; set; }

    public bool IsEmpty => Blocks.Length == 0;
    
    #endregion

    #region Binary

    public override void WriteBinary(BinaryWriter bw)
    {
        byte version = 3;
        bw.Write((byte)version);

        base.WriteBinary(bw);

        bw.Write(Number != null);
        if (Number != null) bw.Write(Number);

        bw.Write((Int32)PageIndex);

        bw.Write(Blocks.Length);
        bw.Write(Fonts.Count);
        bw.Write(Images.Count);
        bw.Write(CountImagesClassifiedAsOcr);

        foreach (var block in Blocks) block.WriteToBinary(bw, version);

        foreach (var font in Fonts) font.WriteToBinary(bw, version);

        foreach (var image in Images) image.WriteToBinary(bw, version);
    }

    public override void ReadBinary(BinaryReader br)
    {
        var v = br.ReadByte();
        if (v != 3)
            throw new UnknownOcrFormat($"Version '{v}' not supported for {nameof(OcrPage)}!");

        base.ReadBinary(br);

        var hasNumber = br.ReadBoolean();
        if (hasNumber) Number = br.ReadString();

        PageIndex = br.ReadInt32();

        var blockCount = br.ReadInt32();
        if (blockCount > 10000)
            throw new UnknownOcrFormat($"Block count {blockCount} greater than 10000!");

        var fontCount = br.ReadInt32();
        if (fontCount > 10000)
            throw new UnknownOcrFormat($"Font count {fontCount} greater than 10000!");

        var imageCount = br.ReadInt32();
        if (imageCount > 65535)
            throw new UnknownOcrFormat($"Image count {imageCount} greater than 65535!");

        CountImagesClassifiedAsOcr = br.ReadInt32();

        for (int blockIndex = 1; blockIndex <= blockCount; blockIndex++)
        {
            var block = new OcrBlock();
            block.ReadFromBinary(br, v);
            AddBlock(block);
        }

        for (int fontIndex = 1; fontIndex <= fontCount; fontIndex++)
        {
            var font = new OcrFont();
            font.ReadFromBinary(br, v);
            AddFont(font);
        }

        for (int imageIndex = 0; imageIndex < imageCount; imageIndex++)
        {
            var image = new OcrImage();
            image.ReadFromBinary(br, v);
            AddImage(image);
        }
    }


    #endregion

    #region Blocks

    private List<OcrBlock> _Blocks = new();
    public OcrBlock[] Blocks => _Blocks.ToArray();
    public int BlocksCount { get => _Blocks.Count; }

    public void RemoveBlock(OcrBlock oBlock)
    {
        _Blocks.Remove(oBlock); 
    }

    public void RemoveBlocks(IEnumerable<OcrBlock> oBlocks)
    {
        foreach (var oBlock in oBlocks) _Blocks.Remove(oBlock); 
    }

    public OcrBlock AddBlock(OcrBlock oBlock) 
    {
        _Blocks.Add(oBlock);
        return oBlock;
    }

    public override void Compile()
    {
        var first = true;
        Int16 index = 0;
        var blocksToRemove = new List<OcrBlock>();
        foreach (var oBlock in _Blocks.ToArray())
        {
            oBlock.Compile();

            if (oBlock.Lines.Count == 0)
            {
                blocksToRemove.Add(oBlock);
                continue;
            }

            oBlock.Page = this;
            oBlock.BlockIndex = index++;

            if (first)
            {
                this.TextDirection = oBlock.TextDirection;
                this.Rect = oBlock.Rect;
                first = false;
            }
            else
            {
                this.Rect = this.Rect.Union(oBlock.Rect);
            }
        }
        if (blocksToRemove.Count > 0)
            RemoveBlocks(blocksToRemove);
    }

    #endregion

    #region Fonts
    public short FontsCount { get => (short)_fonts.Count; }
    public IReadOnlyList<OcrFont> Fonts => _fonts; private List<OcrFont> _fonts = new();
    public OcrFont AddFont(OcrFont font)
    {
        lock (_fonts)
        {
            var index = _fonts.Count;
            _fonts.Add(font);
            font.Page = this;
            font.FontIndex = index;
        }
        return font;
    }

    public short GetFontIndex(string? name)
    {
        if (name == null) return -1;

        for (short i = 0; i < Fonts.Count; i++)
        {
            var fontFamily = Fonts[i];
            if (fontFamily.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        var newFontFamily = new OcrFont() { Name = name };
        AddFont(newFontFamily);
        return FontsCount;
    }

    public OcrFont? AccessFont(string? name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        foreach (var fontFamily in Fonts.ToArray())
        {
            if (fontFamily.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return fontFamily;
        }

        var newFontFamily = new OcrFont() { Name = name };
        AddFont(newFontFamily);
        return newFontFamily;
    }

    #endregion

    #region Images

    public int ImagesCount { get => _images.Count; }
    public IReadOnlyList<OcrImage> Images => _images; private List<OcrImage> _images = new();

    public OcrImage AddImage(OcrImage image)
    {
        lock (_images)
        {
            var index = _images.Count;
            _images.Add(image);
            image.OwnerPage = this;
            image.ImageIndex = index;
        }
        return image;
    }

    #endregion

    #region Symbols

    [JsonIgnore]    public CRect Bounds
    {
        get
        {
            var bounds = new CRect();
            foreach (var block in Blocks)
            {
                bounds = bounds.Union(block.Bounds);
            }
            return bounds;
        }
    }

    public int SymbolsCount
    {
        get
        {
            var count = 0;
            foreach (var block in Blocks)
            {
                count += block.SymbolsCount;
            }
            return count;
        }
    }

    [JsonIgnore] public IEnumerable<OcrSymbol> Symbols
    {
        get
        {
            foreach (var block in Blocks)
            {
                var symbols = block.Symbols;
                foreach (var symbol in symbols)
                    yield return symbol;
            }
        }
    }

    public SkipKeyValue? MustSkip { get; set; }
    public string? PageNumber { get; set; }

    public string DebugPath(OcrDocument document)
    {
        //if (Document == null)
        //    throw new OcrException($"Page '{PageIndex}' does not belong to a document!");

        var oPath = document.Path;
        if (oPath == null)
            throw new OcrException($"Page '{PageIndex}' Document does not have a path!");

        oPath = $"{oPath}.{(PageIndex + 1):000}.debug.png";
        return oPath;
    }
    
    public string OldDebugPath(OcrDocument document)
    {
        //if (Document == null)
        //    throw new OcrException($"Page '{PageIndex}' does not belong to a document!");

        var oPath = document.Path;
        if (oPath == null)
            throw new OcrException($"Page '{PageIndex}' Document does not have a path!");

        oPath = $"{oPath}.{PageIndex + 1}.debug.png";
        return oPath;
    }

    public IEnumerable<OcrSymbol> FindSymbols(string text, CRect bounds)
    {
        foreach (OcrBlock block in Blocks)
        {
            var symbols = block.FindSymbols(text, bounds);
            foreach (var symbol in symbols)
                yield return symbol;
        }
    }

    public void Rotate(double rotation, CPoint centerPoint)
    {
        if (rotation.Almost(0f)) return;

        var t = new CTheory { Rotation = rotation, Origin = centerPoint };

        foreach (var symbol in Symbols)
        {
            symbol.Rect = t.AppliedMoveScaleRotate(symbol.Rect);
        }
    }

    #endregion

    public void SetSymbols(CRect rect, OcrColor color)
    {
        var result = ExtractSymbolsAndText(rect);
        if (result?.Symbols == null) return;

        foreach (var symbol in result.Symbols)
        {
            symbol.Color = color;
        }
    }

    public void SetSymbols(CRect rect, short status)
    {
        var result = ExtractSymbolsAndText(rect);
        if (result?.Symbols == null) return;

        foreach (var symbol in result.Symbols)
        {
            symbol.Status = status;
        }
    }

    public IEnumerable<OcrLine> EnumerateLines(CRect rect)
    {
        foreach (var block in Blocks)
        {
            if (!block.Rect.IntersectsWith(rect)) continue;
            foreach (var line in block.Lines)
            {
                if (!line.Rect.IntersectsWith(rect)) continue;
                yield return line;
            }
        }
    }

    public IEnumerable<OcrWord> EnumerateWords(CRect rect)
    {
        foreach (var block in Blocks)
        {
            if (!block.Rect.IntersectsWith(rect)) continue;
            foreach (var line in block.Lines)
            {
                if (!line.Rect.IntersectsWith(rect)) continue;
                foreach (var word in line.Words)
                {
                    if (!word.Rect.IntersectsWith(rect)) continue;
                    yield return word;
                }
            }
        }
    }

    //public OcrResult? ExtractSymbolsInside(CRect rect)
    //{
    //    var symbols = new List<OcrSymbol>();
    //    var sb = new StringBuilder();

    //    foreach (var block in Blocks)
    //    {
    //        if (!block.Rect.Inside(rect)) continue;

    //        foreach (var line in block.Lines)
    //        {
    //            if (!line.Rect.Inside(rect)) continue;

    //            foreach (var word in line.Words)
    //            {
    //                if (!word.Rect.Inside(rect)) continue;

    //                bool beginWord = true;
    //                foreach (var symbol in word.Symbols)
    //                {
    //                    if (!symbol.Rect.Inside(rect)) continue;
    //                    if (beginWord && sb.Length > 0) sb.Append(' ');

    //                    beginWord = false;
    //                    symbols.Add(symbol);
    //                    sb.Append(symbol.Text);
    //                }
    //            }
    //        }
    //    }
    //    if (symbols.Count > 0)
    //        return new(symbols, sb.ToString());
    //    else
    //        return null;
    //}

    public IEnumerable<OcrPhrase> ExtractLines(CRect searchRect, float maxSymbolHeight = DEFAULT_MAX_SYMBOL_HEIGHT, OcrColor? color = null)
    {
        var actualSymbolHeight = 28f;

        var oPage = this;

        var rect = searchRect;
        rect.Height = actualSymbolHeight; // Math.Min(rect.Height, maxSymbolHeight);
        var top = rect.Top;

        var line = oPage.ExtractSymbolsAndText(rect, maxSymbolHeight: maxSymbolHeight, color: color);
        while (top > 0 && line != null)
        {
            top = line.Bounds.Top;
            rect.Height = line.Symbols.Bounds().Height;
            rect.Top = top - rect.Height - 2;
            var previousLine = oPage.ExtractSymbolsAndText(rect, maxSymbolHeight: maxSymbolHeight, color: color);
            if (previousLine == null) break;
            if (previousLine.Bounds.Top < searchRect.Top) break;
            line = previousLine;
        }

        rect.Top = top;
        rect.Height = maxSymbolHeight;

        while (rect.Top < searchRect.Bottom)
        {
            if (line == null)
            {
                rect.Top += maxSymbolHeight + 2;
                rect.Height = actualSymbolHeight;
                line = oPage.ExtractSymbolsAndText(rect, maxSymbolHeight: maxSymbolHeight, color: color);

                if (line != null)
                {
                    rect.Top = line!.Bounds.Top;
                    line = oPage.ExtractSymbolsAndText(rect, maxSymbolHeight: maxSymbolHeight, color: color);
                }
            }
            else
            {
                if (line.Bounds.Bottom > searchRect.Bottom) break;

                yield return line;

                rect.Top = line.Bounds.Bottom + 4;
                rect.Height = actualSymbolHeight;

                line = oPage.ExtractSymbolsAndText(rect, maxSymbolHeight: maxSymbolHeight, color: color);
                //if (line != null)
                //{
                //    rect.Top = line!.Bounds.Top;
                //    line = oPage.ExtractSymbolsAndText(rect, maxSymbolHeight: maxSymbolHeight, color: color);
                //}
            }

        }
    }

    public IEnumerable<OcrPhrase> ExtractLines(CRect searchRect, int lineNumber, float maxSymbolHeight = DEFAULT_MAX_SYMBOL_HEIGHT, OcrColor? color = null)
    {
        var oPage = this;

        var top = searchRect.Top;
        var line = oPage.ExtractSymbolsAndText(searchRect, maxSymbolHeight: maxSymbolHeight, color: color);
        while (line != null)
        {
            top = line.Bounds.Top;
            if (lineNumber-- > 0)
            {
                searchRect.Top = top - searchRect.Height - 2;
                searchRect.Height = line.Bounds.Height;
                line = oPage.ExtractSymbolsAndText(searchRect, maxSymbolHeight: maxSymbolHeight, color: color);
            }
            else
                line = null;
        }

        searchRect.Top = top;
        line = oPage.ExtractSymbolsAndText(searchRect, maxSymbolHeight: maxSymbolHeight, color: color);
        while (line != null)
        {
            yield return line;
            searchRect.Top = line.Bounds.Bottom + 4;
            line = oPage.ExtractSymbolsAndText(searchRect, maxSymbolHeight: maxSymbolHeight, color: color);
        }
    }

    public string? ExtractText(CRect rect, CTheory? theory = null, string? validCharacters = null, float maxSymbolHeight = DEFAULT_MAX_SYMBOL_HEIGHT)
    {
        var result = ExtractSymbolsAndText(rect, theory, validCharacters, maxSymbolHeight);
        return result?.Text;
    }

    public List<OcrSymbol>? ExtractSymbols(CRect rect, CTheory? theory = null, string? validCharacters = null, float maxSymbolHeight = DEFAULT_MAX_SYMBOL_HEIGHT)
    {
        var result = ExtractSymbolsAndText(rect, theory, validCharacters, maxSymbolHeight);
        return result?.Symbols;
    }

    public OcrPhrase? ExtractSymbolsAndText(CRect rect, CTheory? theory = null, string? validCharacters = null, float maxSymbolHeight = DEFAULT_MAX_SYMBOL_HEIGHT, OcrColor? color = null)
    {
        if (theory != null)
            rect = theory.AppliedMoveScaleRotate(rect);

        var symbols = new List<OcrSymbol>();
        var sb = new StringBuilder();
        OcrSymbol? lastSymbol = null;

        foreach (var block in Blocks)
        {
            if (!block.Rect.IntersectsWith(rect)) continue;

            foreach (var line in block.Lines)
            {
                if (!line.Rect.IntersectsWith(rect)) continue;

                foreach (var word in line.Words)
                {
                    if (!word.Rect.IntersectsWith(rect)) continue;

                    bool beginWord = true;
                    foreach (var symbol in word.Symbols)
                    {
                        if (!symbol.Rect.IntersectsWith(rect)) continue;
                        if (symbol.Rect.Height > maxSymbolHeight) continue;

                        if (beginWord && sb.Length > 0 && sb[sb.Length - 1] != ' ')
                            if (validCharacters == null || validCharacters.Contains(' '))
                            {
                                if (lastSymbol == null ||
                                    lastSymbol.Rect.Right < symbol.Rect.Left - 1)
                                {
                                    sb.Append(' ');
                                }
                            }
                        beginWord = false;

                        if (color != null && symbol.Color != color) continue;
                        //if (color != null && symbol.FillColor != color) continue;
                        //if (color != null && symbol.StrokeColor != color) continue;

                        if (validCharacters != null && !validCharacters.Contains(symbol.Text)) continue;

                        //foreach (var es in symbols)
                        //{
                        //    if (es.Rect.Inside(symbol.Rect.Inflated(2, 2))) continue;
                        //}
                        //if (symbol.BaseLineBegin.Y != symbol.BaseLineEnd.Y) continue;

                        sb.Append(symbol.Text);
                        symbols.Add(symbol);

                        lastSymbol = symbol;
                    }
                }
            }
        }

        if (symbols.Count > 0)
        {
            return new(symbols, sb.ToString());
        }
        else
            return null;
    }

    public CRect FindBoundsForSymbolsOrdered(CRect rect)
    {
        CRect bounds = new();

        foreach (var block in Blocks)
        {
            if (!block.Rect.IntersectsWith(rect)) continue;
            foreach (var line in block.Lines)
            {
                if (!line.Rect.IntersectsWith(rect)) continue;
                foreach (var word in line.Words)
                {
                    if (!word.Rect.IntersectsWith(rect)) continue;
                    foreach (var symbol in word.Symbols)
                    {
                        if (!symbol.Rect.IntersectsWith(rect)) continue;
                        bounds = bounds.Union(symbol.Rect);
                    }
                }
            }
        }
        return bounds;
    }

    //public IEnumerable<OcrSymbol> ExtractSymbols(CRect rect)
    //{
    //    if (rect.IsEmpty)
    //        foreach (var symbol in Symbols)
    //            yield return symbol;

    //    foreach (var symbol in Symbols)
    //    {
    //        // Only consider if the character's center point is inside the rectangle
    //        var basePoint = new CPoint((symbol.Rect.Left + symbol.Rect.Width / 2), symbol.Rect.Bottom);

    //        if (rect.IntersectsWith(basePoint)) yield return symbol;

    //        //if (rect.IntersectsWith(centerPoint)) yield return symbol;
    //        //var centerPoint = symbol.Rect.CenterPoint;

    //        // if (symbol.Rect.IntersectsWith(rect)) yield return symbol;
    //    }
    //}

    //public IEnumerable<OcrSymbol> ExtractSymbols(CRect rect, string text)
    //{
    //    foreach (var symbol in Symbols)
    //    {
    //        if (symbol.Text != text) continue;

    //        // Only consider if the character's center point is inside the rectangle
    //        var centerPoint = symbol.Rect.CenterPoint;
    //        if (rect.IntersectsWith(centerPoint)) yield return symbol;

    //        // if (symbol.Rect.IntersectsWith(rect)) yield return symbol;
    //    }
    //}

    #region Resize

    public void ResizeTo2100()
    {
        var w = Size.Width;
        var h = Size.Height;
        if (w < h)
            ResizeWidth(2100f);
        else
            ResizeHeight(2100f);
    }

    public void ResizeWidth(double width)
    {
        var height = width * this.Size.Height / this.Size.Width;
        Resize(width, height);
    }

    public void ResizeHeight(double height)
    {
        var width = height * this.Size.Width / this.Size.Height;
        Resize(width, height);
    }

    private void Resize(double x, double y)
    {
        var ratioX = x / this.Size.Width;
        var ratioY = y / this.Size.Height;
        if (ratioX == 1 && ratioY == 1) return;

        foreach (var image in Images)
        {
            var r = image.Rect;
            image.Rect = new CRect(r.Left * ratioX, r.Top * ratioY, r.Width * ratioX, r.Height * ratioY);
        }

        foreach (var symbol in Symbols)
        {
            var r = symbol.Rect;
            symbol.Rect = new CRect(r.Left * ratioX, r.Top * ratioY, r.Width * ratioX, r.Height * ratioY);
            symbol.Size = symbol.Rect.Size;
            symbol.SpaceWidth = (float)(symbol.SpaceWidth * ratioX);
            symbol.FontSize = (float)(symbol.FontSize * ratioX);
        }

        this.Size = new(this.Size.Width * ratioX, this.Size.Height * ratioY);

        this.Rect = this.Bounds;
    }

    #endregion

    public override void ApplyMoveScaleRotate(CTheory theory)
    {
        base.ApplyMoveScaleRotate(theory);
        foreach (var image in Images)
        {
            image.ApplyMoveScaleRotate(theory);
        }

        foreach (var block in Blocks)
        {
            block.ApplyMoveScaleRotate(theory);
        }
    }

    public void RemoveSymbol(OcrSymbol oSymbol)
    {
        foreach (var oBlock in Blocks)
        {
            oBlock.RemoveSymbol(oSymbol);
            oBlock.Rect = oBlock.Bounds;
        }
    }

    /// <summary>
    /// Creates a sample page containing a line of text having words "ABC" and "XYZ",
    /// then rotates it around its center.
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static OcrPage CreateSamplePage1(double? rotation = null)
    {
        // A:100, B:110, C:120, X:140, Y:150, Z:160
        var line = OcrLine.Construct(text: "ABC XYZ", start: new CPoint(100, 100), symbolSize: new CSize(10, 10));
        var block = OcrBlock.Construct(line);
        var page = OcrPage.Construct(block);

        if (rotation != null) page.Rotate(rotation.Value);

        page.Compile();
        return page;
    }

    ///// <summary>Create a sample page filled with assorted characters and known specific phrases.</summary>
    ///// <param name="rotation"></param>
    ///// <returns></returns>
    //public static OcrPage CreateSamplePage2(double? rotation = null)
    //{
    //    var r = new Random();

    //    var page = new OcrPage();

    //    var block = page.AddBlock(new OcrBlock());

    //    var symbolWidth = 10;
    //    var symbolHeight = 10;  
    //    var spacingX = 2;
    //    var spacingY = 5;

    //    // Create 10 rows of random text
    //    for (int row = 0; row < 10; row++)
    //    {
    //        var top = row * (symbolHeight+spacingY);

    //        var line = new OcrLine();
    //        var columns = r.Next(0, 100);
    //        var word = line.AddWord( new OcrWord());
    //        for (int column = 0; column < columns; column++)
    //        {
    //            var left = column * (symbolWidth+spacingX);
    //            if (!isSpace)
    //            {
    //                var character = (char)(r.Next(0, 25) + (int)'A');
    //                var symbol = new OcrSymbol("" + character, new CRect(left, top, symbolWidth, symbolHeight));
    //                word.AddSymbol(symbol);
    //            }
    //            var isSpace = (r.Next(0, 7) == 0);
    //            if (isSpace) word = new OcrWord();
    //        }
    //        if (line.Words.Count > 0)
    //            block.AddLine(line);
    //    }

    //    if (rotation != null) page.Rotate(rotation.Value);
    //    page.Compile();
    //    return page;
    //}
}