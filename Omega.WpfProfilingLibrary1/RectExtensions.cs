using Jem.CommonLibrary22;

namespace Omega.WpfProfilingLibrary1;

public static class RectExtensions
{
    public static Rect ToRect(this CRect rect)
    {
        return new Rect
        {
            X = rect.Left,
            Y = rect.Top,
            Width = rect.Width,
            Height = rect.Height,
        };
    }

    public static bool HasWidthAndHeight(this Rect rect)
    {
        if (rect.IsEmpty) return false;
        if (Math.Round(rect.Width, 8) == 0f) return false;
        if (Math.Round(rect.Height, 8) == 0f) return false;
        return true;
    }

    public static string ToStringRounded(this Rect rect, int maxDecimals = 2)
    {
        var x = Math.Round(rect.X, maxDecimals);
        var y = Math.Round(rect.Y, maxDecimals);
        var w = Math.Round(rect.Width, maxDecimals);
        var h = Math.Round(rect.Height, maxDecimals);

        return $"{x},{y},{w},{h}";
    }

    public static Point CenterPoint(this Rect rect)
    {
        return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
    }
}
