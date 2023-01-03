namespace Jem.CommonLibrary22;

public struct CPoint : IEquatable<CPoint>
{
    public static CPoint Empty { get; } = new CPoint();

    [DebuggerStepThrough]
    public CPoint(double x, double y)
    {
        this.X = x;
        this.Y = y;
    }

    [JsonIgnore]
    public bool AtOrigin => X.Almost(0) && Y.Almost(0);

    [JsonIgnore]
    public bool IsEmpty =>
        (X.Almost(0) || double.IsInfinity(X) || double.IsNaN(X)) ||
        (Y.Almost(0) || double.IsInfinity(Y) || double.IsNaN(Y));

    [JsonIgnore]
    public bool IsNotEmpty => !IsEmpty;

    public double X { get; set; }

    public double Y { get; set; }

    public override string ToString() => $"{X},{Y}";

    public string ToStringRounded() => $"{X:0.##},{Y:0.##}";

    public bool Equals(CPoint other)
    {
        return other.X.Almost(X) && other.Y.Almost(Y);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not CPoint)
        {
            return false;
        }
        var other = (CPoint)obj;
        return Equals(other);
    }

    public override int GetHashCode() => X.GetHashCode() >> 16 + Y.GetHashCode();

    public static bool operator ==(CPoint one, CPoint two)
    {
        return one.X.Almost(two.X) && one.Y.Almost(two.Y);
    }

    public static bool operator !=(CPoint one, CPoint two)
    {
        return !one.X.Almost(two.X) || !one.Y.Almost(two.Y);
    }

    public (int x, int y) Ceiling()
    {
        return ((int)Math.Ceiling(X), (int)Math.Ceiling(Y));
    }


    public static CPoint ReadFromBinary(BinaryReader br)
    {
        var x = br.ReadDouble();
        var y = br.ReadDouble();
        return new CPoint(x, y);
    }

    public void WriteToBinary(BinaryWriter bs)
    {
        bs.Write((double)X);
        bs.Write((double)Y);
    }
}

public static class CPointExtensions
{
    public static CPoint ReadCPoint(this BinaryReader br) => CPoint.ReadFromBinary(br);
    public static void Write(this BinaryWriter bw, CPoint point) => point.WriteToBinary(bw);

}
