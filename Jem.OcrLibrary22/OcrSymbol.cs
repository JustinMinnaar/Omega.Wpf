using System.Text.Json.Serialization;

using static System.Net.Mime.MediaTypeNames;

namespace Jem.OcrLibrary22;

public sealed class OcrSymbol : OcrElement
{
    #region ctor

    public OcrSymbol() { }

    public OcrSymbol(string character, CRect rect, CSize size) : base(character, rect, size) { }

    public OcrSymbol(string text, CRect rect) : base(text, rect, rect.Size) {  }

    public OcrSymbol(string text, CRect rect, CSize size,
        short fontFamilyIndex, float fontSize, float spaceWidth,
        short status, OcrColor color, float confidence,
        CPoint baseLineBegin, CPoint baseLineEnd) : base(text, rect, size)
    {
        FontIndex = fontFamilyIndex;
        FontSize = fontSize;
        SpaceWidth = spaceWidth;
        Status = status;
        Color = color;
        Confidence = confidence;
        BaseLineBegin = baseLineBegin;
        BaseLineEnd = baseLineEnd;
    }

    public OcrSymbol(OcrSymbol oldSymbol) : base(oldSymbol)
    {
        this.FontIndex = oldSymbol.FontIndex;
        this.FontSize = oldSymbol.FontSize;
        this.SpaceWidth = oldSymbol.SpaceWidth;
        this.Color = oldSymbol.Color;
        this.Status = oldSymbol.Status;
        this.Confidence = oldSymbol.Confidence;

        this.BaseLineBegin = oldSymbol.BaseLineBegin;
        this.BaseLineEnd = oldSymbol.BaseLineEnd;

        this.HorizontalScaling = oldSymbol.HorizontalScaling;
        this.WordSpacing = oldSymbol.WordSpacing;
        this.Rise = oldSymbol.Rise;
        this.Leading = oldSymbol.Leading;
        this.StrokeColor = oldSymbol.StrokeColor;
        this.FillColor = oldSymbol.FillColor;
    }

    #endregion

    #region Owner

    [JsonIgnore] public OcrWord? Word { get; internal set; }
    public int SymbolIndex { get; internal set; }

    #endregion

    #region Properties

    public Int16 FontIndex { get; set; }
    public float FontSize { get; set; }
    public float SpaceWidth { get; set; }
    public short Status { get; set; }
    public OcrColor Color { get; set; }
    public CPoint BaseLineBegin { get; set; }
    public CPoint BaseLineEnd { get; set; }

    public float HorizontalScaling { get; set; }
    public float WordSpacing { get; set; }
    public float Rise { get; set; }
    public float Leading { get; set; }
    public OcrColor StrokeColor { get; set; }
    public OcrColor FillColor { get; set; }

    #endregion

    #region Binary

    internal void WriteToBinary(BinaryWriter bw, byte version)
    {
        base.WriteBinary(bw);

        if (version >= 1) WriteToBinary1(bw);
        if (version >= 2) WriteToBinary2(bw);
    }

    internal void ReadFromBinary(BinaryReader br, byte version)
    {
        base.ReadBinary(br);

        if (version >= 1) ReadFromBinary1(br);
        if (version >= 2) ReadFromBinary2(br);
    }

    private void ReadFromBinary1(BinaryReader br)
    {
        Text = br.ReadString();

        SymbolIndex = br.ReadInt32();

        FontIndex = br.ReadInt16();
        FontSize = br.ReadSingle();
        SpaceWidth = br.ReadSingle();

        Color = br.ReadOcrColor();

        Status = br.ReadInt16();
        Confidence = br.ReadSingle();
    }

    private void WriteToBinary1(BinaryWriter bw)
    {
        bw.Write(Text);

        bw.Write((Int32)SymbolIndex);

        bw.Write((Int16)FontIndex);
        bw.Write((Single)FontSize);
        bw.Write((Single)SpaceWidth);

        bw.WriteToBinary(Color);

        bw.Write((Int16)Status);
        bw.Write((Single)Confidence);
    }

    private void ReadFromBinary2(BinaryReader br)
    {
        // pdf specific information
        BaseLineBegin = br.ReadCPoint();
        BaseLineEnd = br.ReadCPoint();

        HorizontalScaling = br.ReadSingle();
        WordSpacing = br.ReadSingle();
        Rise = br.ReadSingle();
        Leading = br.ReadSingle();

        StrokeColor = br.ReadOcrColor();
        FillColor = br.ReadOcrColor();
    }

    private void WriteToBinary2(BinaryWriter bw)
    {
        // pdf specific information
        bw.Write(BaseLineBegin);
        bw.Write(BaseLineEnd);

        bw.Write((Single)HorizontalScaling);
        bw.Write((Single)WordSpacing);
        bw.Write((Single)Rise);
        bw.Write((Single)Leading);

        bw.WriteToBinary(StrokeColor);
        bw.WriteToBinary(FillColor);
    }

    #endregion

    #region ToText

    public override void ToText(StringBuilder sb)
    {
        sb.Append(Text);
    }

    public bool Matches(char actual, bool isMask)
    {
        var expecting = Text[0];
        if (!isMask) return expecting == actual;

        var match = expecting switch
        {
            '*' => true,
            '#' => char.IsDigit(actual) || ".,+- ".Contains(actual),
            'U' => char.IsUpper(actual),
            'L' => char.IsLower(actual),
            'A' => char.IsLower(actual),
            _ => expecting == actual
        };
        return match;
    }

    #endregion

    public string ToStringDebug
    {
        get => $"{Text} ({Rect}) " +
               $"font:{FontIndex}/{FontSize} space:{SpaceWidth} spacing:{WordSpacing} " +
               $"status:{Status} color:{Color} " +
               $"index:{SymbolIndex} confidence:{Confidence}";
    }

    public override void Compile()
    {
    }


}
