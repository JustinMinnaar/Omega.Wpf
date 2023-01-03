using System.Text.Json.Serialization;

namespace Jem.OcrLibrary22;

public sealed class OcrFont
{
    public override string ToString() => Name;

    [JsonIgnore] public OcrPage? Page { get; internal set; }
    public int FontIndex { get; internal set; }

    public string Name { get; set; } = string.Empty;

    internal void WriteToBinary(BinaryWriter bw, byte version)
    {
        bw.Write(Name);
    }

    internal void ReadFromBinary(BinaryReader br, byte version)
    {
        Name = br.ReadString();
    }
}