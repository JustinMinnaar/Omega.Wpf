namespace Jem.CommonLibrary22;

[DebuggerDisplay("{TopLeft} : {TopRight} : {BottomLeft} : {BottomRight}")]
public struct Bounds
{
    public CPoint TopLeft { get; set; }
    public CPoint TopRight { get; set; }
    public CPoint BottomLeft { get; set; }
    public CPoint BottomRight { get; set; }

    public CRect ToRect()
    {
        var top = Math.Min(TopLeft.Y, TopRight.Y);
        var bottom = Math.Max(BottomLeft.Y, BottomRight.Y);
        var left = Math.Min(TopLeft.X, BottomLeft.X);
        var right = Math.Max(TopRight.X, BottomRight.X);

        return new CRect(left, top, right - left, bottom - top);
    }
}
