namespace Omega.WpfProfilingLibrary1;

public static class PointExtensions
{
    public static string ToStringRounded(this Point p, int maxDecimals = 2)
    {
        var x = Math.Round(p.X, maxDecimals);
        var y = Math.Round(p.Y, maxDecimals);

        return $"{x},{y}";
    }
}
