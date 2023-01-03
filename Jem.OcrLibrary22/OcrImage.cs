using System.Text.Json.Serialization;

namespace Jem.OcrLibrary22;

public sealed class OcrImage
{
    #region Owner

    [JsonIgnore] public OcrPage? OwnerPage { get; internal set; }

    public int ImageIndex { get; internal set; }

    #endregion

    #region Properties

    public string Path { get; set; } = String.Empty;
    public string Checksum { get; set; } = String.Empty;
    public CRect Rect { get; set; }
    public bool OcrDataFound { get; set; }

    #endregion

    #region Binary

    internal void WriteToBinary(BinaryWriter bw, byte version)
    {
        bw.Write((byte)version);

        bw.Write(Path);
        bw.Write(Checksum);
        Rect.WriteToBinaryFloat(bw);
    }

    internal void ReadFromBinary(BinaryReader br, byte version)
    {
        _ = br.ReadByte();
        if (version < 2)
            throw new UnknownOcrFormat($"Version '{version}' not supported for {nameof(OcrImage)}!");

        Path = br.ReadString();
        Checksum = br.ReadString();
        if (version == 2)
        {
            Rect = CRect.ReadFromBinaryFloat(br);
        }
        else
        {
            Rect = new CRect(-1, -1, br.ReadSingle(), br.ReadSingle());
        }
    }

    #endregion

    public void ApplyMoveScaleRotate(CTheory theory)
    {
        Rect = theory.AppliedMoveScaleRotate(Rect);
    }

}