namespace Jem.CommonLibrary22;

public struct CSize : IEquatable<CSize>
{
    public static CSize Empty { get; } = new CSize();

    [DebuggerStepThrough]
    public CSize(double width, double height)
    {
        this.Width = width;
        this.Height = height;
    }

    public static CSize operator +(CSize a, CSize b)
    {
        return new(a.Width + b.Width, a.Height + b.Height);
    }

    public static CSize operator -(CSize a, CSize b)
    {
        return new(a.Width - b.Width, a.Height - b.Height);
    }

    public double Width { get; set; }

    public double Height { get; set; }

    public bool IsEmpty => Width != 0 || Height != 0;

    public override string ToString() => $"{Width},{Height}";

    public string ToStringRounded() => $"{Width:0.##},{Height:0.##}";

    public bool Equals(CSize other)
    {
        return other.Width.Almost(Width) && other.Height.Almost(Height);
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is CSize))
        {
            return false;
        }
        var other = (CSize)obj;
        return Equals(other);
    }

    public override int GetHashCode() => Width.GetHashCode() >> 16 + Height.GetHashCode();

    public static bool operator ==(CSize one, CSize two)
    {
        return one.Width.Almost(two.Width) && one.Height.Almost(two.Height);
    }

    public static bool operator !=(CSize one, CSize two)
    {
        return !one.Width.Almost(two.Width) || !one.Height.Almost(two.Height);
    }

    public CSize Reverse()
    {
        return new CSize(-this.Width, -this.Height);
    }

    public CSize Min(CSize maximum)
    {
        return new CSize
        {
            Width = Math.Min(Width, maximum.Width),
            Height = Math.Min(Height, maximum.Height)
        };
    }

    public CSize Max(CSize minimum)
    {
        return new CSize
        {
            Width = Math.Max(Width, minimum.Width),
            Height = Math.Max(Height, minimum.Height)
        };

    }

    public (int x, int y) Ceiling()
    {
        return ((int)Math.Ceiling(this.Width), (int)Math.Ceiling(this.Height));
    }

    public static CSize ReadFromBinaryFloat(BinaryReader br)
    {
        var w = br.ReadSingle();
        var h = br.ReadSingle();
        return new CSize(w, h);
    }

    public static CSize ReadFromBinaryDouble(BinaryReader br)
    {
        var w = br.ReadDouble();
        var h = br.ReadDouble();
        return new CSize(w, h);
    }

    public void WriteToBinaryFloat(BinaryWriter bs)
    {
        bs.Write((float)Width);
        bs.Write((float)Height);
    }

    public void WriteToBinaryDouble(BinaryWriter bs)
    {
        bs.Write((double)Width);
        bs.Write((double)Height);
    }

}