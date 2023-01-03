using System.Diagnostics.CodeAnalysis;

namespace Jem.OcrLibrary22;

public struct OcrColor
{
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is OcrColor other)
            return Equals(other);
        else
            return false;
    }

    public bool Equals(OcrColor other)
    {
        if (other.R != R) return false;
        if (other.G != G) return false;
        if (other.B != B) return false;
        return true;
    }

    public override string ToString() => $"{R},{G},{B}";
    public int R, G, B;

    public OcrColor(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    public static OcrColor ReadFromBinary(BinaryReader br)
    {
        return new OcrColor { R = br.ReadInt32(), G = br.ReadInt32(), B = br.ReadInt32() };
    }

    public void WriteToBinary(BinaryWriter bw)
    {
        bw.Write((Int32)R);
        bw.Write((Int32)G);
        bw.Write((Int32)B);
    }

    public static bool operator ==(OcrColor left, OcrColor right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(OcrColor left, OcrColor right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
    }
}

public static class OcrColorExtensions
{
    public static OcrColor ReadOcrColor(this BinaryReader br) => OcrColor.ReadFromBinary(br);
    public static void WriteToBinary(this BinaryWriter bw, OcrColor color) => color.WriteToBinary(bw);

}