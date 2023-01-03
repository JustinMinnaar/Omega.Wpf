namespace Jem.CommonLibrary22;

public static class CMaths
{
    #region Within

    public static bool InRange(this double value, double minValue, double maxValue)
        => value >= minValue && value <= maxValue;

    public static bool InRange(this int value, int minValue, int maxValue)
        => value >= minValue && value <= maxValue;

    #endregion Within

    #region Almost (EPSILON)

    public static bool Almost(this double value, double otherValue)
      => Math.Round(Math.Abs(value - otherValue), 6) < double.Epsilon;

    #endregion Almost (EPSILON)

    #region Min and Max

    public static T Min<T>(T first, params T[] values) where T : IComparable
    {
        foreach (var value in values) { if (first.CompareTo(value) > 0) first = value; }
        return first;
    }

    public static T Max<T>(T first, params T[] values) where T : IComparable
    {
        foreach (var value in values) { if (first.CompareTo(value) < 0) first = value; }
        return first;
    }

    #endregion Min and Max

    #region Hypotenuse

    public static double Hypotenuse(this double x, double y)
    {
        return (double)Math.Sqrt(x * x + y * y);
    }

    public static double DistanceBetween(CPoint p1, CPoint p2)
    {
        double xDistance = Math.Abs((double)p2.X - p1.X);
        double yDistance = Math.Abs((double)p2.Y - p1.Y);
        return Hypotenuse(xDistance, yDistance);
    }

    #endregion Hypotenuse

    #region RotatePoint

    public static CPoint RotatePoint(this CPoint p, double angle)
    {
        // Find the distance that the character is from (0, 0)
        // Determine the angle that the character is from (0, 0)
        // Increment the angle by the specified number of degrees
        // Calculate the new position of the character relative to (0, 0)
        // return the new value

        var distanceFrom00 = CMaths.Hypotenuse(p.X, p.Y);
        var originalAngle = CMaths.AngleFromZero(p);
        var newAngle = originalAngle + angle;
        var newX = (double)Math.Round(CMaths.CalculateTriangleHeight(distanceFrom00, newAngle), 8);
        var newY = (double)Math.Round(CMaths.CalculateTriangleBase(distanceFrom00, newAngle), 8);

        return new CPoint(newX, newY);
    }

    #endregion RotatePoint

    #region CalculateTriangle...

    public static double CalculateTriangleBase(double distanceFrom00, double angle)
    {
        var radians = DegreesToRadians(angle);
        return (double)(-Math.Cos(radians) * distanceFrom00);
    }

    public static double CalculateTriangleHeight(double distanceFrom00, double angle)
    {
        var radians = DegreesToRadians(angle);
        return (double)(+Math.Sin(radians) * distanceFrom00);
    }

    #endregion CalculateTriangle...

    #region Radians

    /// <summary>Converts radians to degrees.</summary>
    /// <param name="radians"></param>
    /// <returns>The radians as degrees</returns>
    public static double RadiansToDegrees(double radians)
    {
        return (double)(radians * (180f / Math.PI));
    }

    /// <summary>Converts degrees to radians.</summary>
    /// <param name="degrees"></param>
    /// <returns>The degrees as radians</returns>
    public static double DegreesToRadians(double degrees)
    {
        return (degrees * ((double)Math.PI / 180f));
    }

    #endregion Radians

    #region Angles

    /// <summary>Determines the angle between the centre points of two rectangles</summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns>The angle between the two rectangles</returns>
    public static double AngleBetween(CRect first, CRect second)
    {
        // Calculate the angle between the top/left of each rectangle
        var Left = new CPoint(first.Left + first.Width / 2f, first.Top + first.Height / 2f);
        var Right = new CPoint(second.Left + second.Width / 2f, second.Top + second.Height / 2f);
        return AngleBetween(Left, Right);
    }

    /// <summary>Determines the angle between the centre points of two rectangles.</summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    /// <returns>The angle between the two rectangles, in degrees</returns>
    public static double AngleBetween(CPoint point1, CPoint point2)
    {
        var point = new CPoint(point2.X - point1.X, point2.Y - point1.Y);
        return AngleFromZero(point);
    }

    public static double AngleFromZero(this CPoint point)
    {
        var xDistance = point.X;
        var yDistance = point.Y;

        if (xDistance.Almost(0) && yDistance.Almost(0)) return double.NaN;

        // Are we going diCRectly up or diCRectly down?
        if (yDistance.Almost(0)) { if (xDistance < 0) return 270f; else return 90f; }
        if (xDistance.Almost(0)) { if (yDistance < 0) return 0f; else return 180f; }

        // Calculate the angle inside the triangle
        var tangeant = (double)yDistance / (double)xDistance;
        var radians = (double)Math.Atan(tangeant);
        var degrees = CMaths.RadiansToDegrees(radians);

        if (point.X >= 0) return 90 + degrees;
        else return 270 + degrees;
    }

    #endregion Angles
}