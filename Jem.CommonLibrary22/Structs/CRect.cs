namespace Jem.CommonLibrary22;

using System;
using System.Diagnostics;
using System.Drawing;

/// <summary>
/// Represents a rectangle with left/top and width/height and right/bottom coordinates.
/// This struct was required as the Rect for Windows.Base for WPF does not support negative
/// widths, often required when we draw or calculate CTheory for processing. This also
/// isolates the code from dependency on WPF.
/// </summary>
[Serializable]
public struct CRect : IEquatable<CRect>
{
    public static CRect Empty { get; } = new CRect();

    public static CRect operator +(CRect rect, CRect other)
    {
        return rect.Union(other);
    }

    public static CRect operator +(CRect rect, CPoint pos)
    {
        return new CRect(rect.Left + pos.X, rect.Top + pos.Y, rect.Width, rect.Height);
    }

    public static CRect operator +(CRect rect, CSize size)
    {
        return new CRect(rect.Left, rect.Top, rect.Width + size.Width, rect.Height + size.Height);
    }

    public static CRect operator -(CRect rect, CPoint pos)
    {
        return new CRect(rect.Left - pos.X, rect.Top - pos.Y, rect.Width, rect.Height);
    }

    public static CRect operator -(CRect rect, CSize size)
    {
        return new CRect(rect.Left, rect.Top, rect.Width - size.Width, rect.Height - size.Height);
    }

    [DebuggerStepThrough]
    public CRect(double left, double top, double width, double height)
    {
        // if width is negative, we move left and change to + width
        if (width < 0) { left += width; width = -width; }

        // if height is negative, we move up and change to + height
        if (height < 0) { top += height; height = -height; }

        Left = left;
        Top = top;
        Width = width;
        Height = height;
    }

    [DebuggerStepThrough]
    public CRect(CPoint point, CSize size)
    {
        Left = point.X;
        Top = point.Y;
        Width = size.Width;
        Height = size.Height;
    }

    public override string ToString() => $"l:{Left} t:{Top} w:{Width} h:{Height}";

    public string ToString1() => $"{Left:0.0},{Top:0.0},{Width:0.0},{Height:0.0}";

    public string ToString2() => $"{Left:0.00},{Top:0.00} ,{Width:0.00},{Height:0.00}";

    /// <summary>Returns a System.Drawing.Rectangle.</summary>
    public Rectangle ToRectangle() => new((int)Left, (int)Top, (int)Width, (int)Height);

    public double Left { get; set; }
    public double Top { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    [JsonIgnore]
    public double Right { get => Left + Width; set => Width = value - Left; }

    [JsonIgnore]
    public double Bottom { get => Top + Height; set => Height = value - Top; }

    public bool Contains(CPoint p)
    {
        if (p.X < Left) return false;
        if (p.Y < Top) return false;
        if (p.X > Right) return false;
        if (p.Y > Bottom) return false;

        return true;
    }

    public CPoint CenterPoint
        => new CPoint { X = (Left + Right) / 2, Y = (Top + Bottom) / 2 };

    [DebuggerStepThrough]
    public CRect(float left, float top, float width, float height)
    {
        Left = (float)left;
        Top = (float)top;
        Width = (float)width;
        Height = (float)height;
    }

    public static CRect UnionOf(IEnumerable<CRect> rects)
    {
        var r = new CRect();
        foreach (var rect in rects)
        {
            r = r.Union(rect);
        }
        return r;
    }

    public bool IntersectsWith(CRect other)
    {
        if (other.Left > Right) return false;
        if (other.Right < Left) return false;

        if (other.Top > Bottom) return false;
        if (other.Bottom < Top) return false;

        return true;
    }

    public bool IntersectsWith(CPoint point)
    {
        if (point.X > Right) return false;
        if (point.X < Left) return false;

        if (point.Y > Bottom) return false;
        if (point.Y < Top) return false;

        return true;
    }

    public bool Inside(CRect other)
    {
        if (other.Right < Right) return false;
        if (other.Top > Top) return false;
        if (other.Left > Left) return false;
        if (other.Bottom < Bottom) return false;
        return true;
    }

    [JsonIgnore]
    public bool IsEmpty => (Width.Almost(0) || double.IsInfinity(Width) || double.IsNaN(Width)) ||
                            (Height.Almost(0) || double.IsInfinity(Height) || double.IsNaN(Height));

    [JsonIgnore]
    public bool IsNotEmpty => !IsEmpty;

    public CSize Size => new CSize(this.Width, this.Height);

    public static CRect Parse(string s)
    {
        if (s == null) return new CRect();

        if (s.StartsWith("(") && s.EndsWith(")"))
        {
            s = s.Substring(1, s.Length - 2);
        }

        var parts = s.Split(',');
        if (parts.Length == 4)
        {
            var left = double.Parse(parts[0]);
            var top = double.Parse(parts[1]);
            var width = double.Parse(parts[2]);
            var height = double.Parse(parts[3]);
            return new CRect(left, top, width, height);
        }

        throw new InvalidCastException($"Could not cast \"{s}\" to {nameof(CRect)}. Expected \"(left,top,width,height)\"!");
    }

    public CRect Inflated(CSize size) => Inflated(size.Width, size.Height);

    public CRect Inflated(double width, double height)
    {
        var rect = new CRect
        {
            Left = this.Left - width / 2,
            Top = this.Top - height / 2,
            Width = this.Width + width,
            Height = this.Height + height
        };
        return rect;
    }

    /// <summary>
    /// Increases the size of the CRectangle by the width/height specified, such that the left edge moves half this width left and the top edge moves half this height up.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void Inflate(double width, double height)
    {
        this.Left -= width / 2;
        this.Top -= height / 2;
        this.Width += width;
        this.Height += height;
    }

    /// <summary>Joins the other rectangle with this rectangle. If this rectangle is empty, returns other. If other is empty, returns this.</summary>
    /// <param name="other"></param>
    public CRect Union(CRect other)
    {
        if (this.Width == 0 || this.Height == 0) return other;
        if (other.Width == 0 || other.Height == 0) return this;

        var left = Math.Min(this.Left, other.Left);
        var right = Math.Max(this.Right, other.Right);

        var top = Math.Min(this.Top, other.Top);
        var bottom = Math.Max(this.Bottom, other.Bottom);

        var width = right - left;
        var height = bottom - top;

        var result = new CRect(left, top, width, height);
        return result;
    }

    /// <summary>Return the rectangle bound inside other rectangle.</summary>
    public CRect BoundInside(CRect other)
    {
        if (other.IsEmpty) return CRect.Empty;

        var left = this.Left;
        var top = this.Top;
        var width = this.Width;
        var height = this.Height;

        var diffX = other.Left - left;
        var diffY = top - other.Top;

        // Move the rect right and down until it is inside the other rect
        if (diffX > 0) { left += diffX; }
        if (diffY > 0) { top += diffY; }

        // Make the rect smaller to fit into the other rect
        if (other.Right < this.Right) { this.Width = other.Right - this.Left; }
        if (other.Bottom < this.Bottom) { this.Height = other.Bottom - this.Top; }

        return new CRect(left, top, width, height);
    }

    //public void Move(double x, double y)
    //{
    //    this.Left += x;
    //    this.Top += y;
    //}

    public CRect Moved(double x, double y)
    {
        return new CRect
        {
            Left = this.Left + x,
            Top = this.Top + y,
            Width = this.Width,
            Height = this.Height
        };
    }

    public CPoint TopLeft
    {
        get => new CPoint(Left, Top);
        set
        {
            Left = value.X;
            Top = value.Y;
        }
    }

    public CPoint TopRight => new CPoint(Right, Top);

    public CPoint BottomRight => new CPoint(Right, Bottom);

    public CPoint BottomLeft => new CPoint(Left, Bottom);

    public bool Equals(CRect other)
    {
        return other.Left.Almost(Left) && other.Top.Almost(Top) &&
               other.Width.Almost(Width) && other.Height.Almost(Height);
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is CRect))
        {
            return false;
        }
        var other = (CRect)obj;
        return other.Width == Width && other.Height == Height;
    }

    public override int GetHashCode() => Left.GetHashCode() + Top.GetHashCode() >> 8 + Width.GetHashCode() >> 16 + Height.GetHashCode() >> 24;

    public static bool operator ==(CRect one, CRect two)
    {
        return one.Left == two.Left && one.Top == two.Top && one.Width == two.Width && one.Height == two.Height;
    }

    public static bool operator !=(CRect one, CRect two)
    {
        return one.Left != two.Left || one.Top != two.Top || one.Width != two.Width || one.Height != two.Height;
    }

    public void WriteToBinaryFloat(BinaryWriter bw)
    {
        bw.Write((float)Left);
        bw.Write((float)Top);
        bw.Write((float)Width);
        bw.Write((float)Height);
    }

    public void WriteToBinaryDouble(BinaryWriter bw)
    {
        bw.Write(Left);
        bw.Write(Top);
        bw.Write(Width);
        bw.Write(Height);
    }

    public static CRect ReadFromBinaryFloat(BinaryReader br)
    {
        return new CRect(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
    }

    public static CRect ReadFromBinaryDouble(BinaryReader br)
    {
        return new CRect(br.ReadDouble(), br.ReadDouble(), br.ReadDouble(), br.ReadDouble());
    }

    public (int x, int y) CeilingPoint()
    {
        return ((int)Math.Ceiling(this.Left), (int)Math.Ceiling(this.Top));
    }

    public (int x, int y) CeilingSize()
    {
        return ((int)Math.Ceiling(this.Width), (int)Math.Ceiling(this.Height));
    }
}