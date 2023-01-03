namespace Jem.OcrLibrary22;

public abstract class OcrElement
{
    public OcrElement()
    {

    }

    public OcrElement(string text, CRect rect, CSize size)
    {
        Text = text;
        Rect = rect;
        Size = size;
    }

    protected OcrElement(OcrElement old)
    {
        Text = old.Text;
        Rect = old.Rect;
        Size = old.Size;
        TextDirection = old.TextDirection;
        Confidence = old.Confidence;
    }

    public string Text { get; set; } = string.Empty;
    public CRect Rect { get; set; }
    public CSize Size { get; set; }
    public float Confidence { get; set; }
    public OcrTextDirection TextDirection { get; set; }

    public override string ToString() => ToText();

    #region ToText

    public string ToText()
    {
        var sb = new StringBuilder();
        ToText(sb);
        return sb.ToString();
    }

    public abstract void ToText(StringBuilder sb);

    #endregion

    #region Binary

    public virtual void WriteBinary(BinaryWriter bw)
    {
        bw.Write(ToText());
        WriteBinary2(bw);
    }

    public virtual void WriteBinary2(BinaryWriter bw)
    {
        Rect.WriteToBinaryFloat(bw);
        Size.WriteToBinaryFloat(bw);
        bw.Write((byte)TextDirection);
        bw.Write((float)Confidence);
    }

    public virtual void ReadBinary(BinaryReader br)
    {
        Text = br.ReadString();
        ReadBinary2(br);
    }

    public virtual void ReadBinary2(BinaryReader br)
    {
        Rect = CRect.ReadFromBinaryFloat(br);
        Size = CSize.ReadFromBinaryFloat(br);
        TextDirection = (OcrTextDirection)br.ReadByte();
        Confidence = br.ReadSingle();
    }

    #endregion

    public void SaveToBinaryFile(string path)
    {
        using var bws = File.OpenWrite(path);
        SaveToBinaryStream(bws);
    }

    public void SaveToBinaryStream(Stream stream)
    {
        // note: a 'using' clause would close the stream and we want to leave it open
        var bs = new BinaryWriter(stream);
        WriteBinary(bs);
    }

    public static T LoadFromBinaryStream<T>(Stream stream) where T : OcrElement, new()
    {
        // note: a 'using' clause would close the stream and we want to leave it open
        var br = new BinaryReader(stream);

        var page = new T();
        page.ReadBinary(br);
        return page;
    }

    public virtual void ApplyMoveScaleRotate(CTheory theory)
    {
        Rect = theory.AppliedMoveScaleRotate(Rect);
    }

    public void Move(double x, double y)
    {
        var theory = new CTheory { Move = new(x, y) };
        ApplyMoveScaleRotate(theory);
    }

    public void Scale(double scale)
    {
        var centerPoint = Rect.CenterPoint;
        var theory = new CTheory { Origin = centerPoint, Scaling = scale };
        ApplyMoveScaleRotate(theory);
    }

    public void Rotate(double degrees)
    {
        var centerPoint = Rect.CenterPoint;
        var theory = new CTheory { Origin = centerPoint, Rotation = degrees };
        ApplyMoveScaleRotate(theory);
    }

    public virtual void Compile()
    {
    }
}
