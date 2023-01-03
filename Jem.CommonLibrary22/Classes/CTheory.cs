namespace Jem.CommonLibrary22;

// Must remain a class as code is expecting to pass it to functions that make changes to it
[Nickname("theory")]
public class CTheory
{
    [DebuggerStepThrough]
    public CTheory()
    {
        Move = new();
        Rotation = 0;
        Origin = new();
        Scaling = 1f;
    }

    public CTheory(CTheory? other) : this()
    {
        if (other == null) return;

        this.Move = other.Move;
        this.Rotation = other.Rotation;
        this.Origin = other.Origin;
        this.Scaling = other.Scaling;
    }

    public override string ToString()
        => $"move:{Move.ToStringRounded()} scaling:{Scaling:0.##} rotate:{Rotation} origin:{Origin.ToStringRounded()} ";

    /// <summary>This is the distance from origin to shift X and Y values before rotating around origin.</summary>
    public CSize Move;

    public double Rotation;
    public CPoint Origin;
    public double Scaling = 1f;

    public bool IsEmpty =>
        this.Rotation.Almost(0) &&
        this.Scaling.Almost(1) &&
        this.Move.Width.Almost(0) &&
        this.Move.Height.Almost(0);

    public CRect AppliedMoveScaleRotate(CRect CRect)
    {
        var topLeft = new CPoint(CRect.Left, CRect.Top);
        var topRight = new CPoint(CRect.Right, CRect.Top);
        var botRight = new CPoint(CRect.Right, CRect.Bottom);
        var botLeft = new CPoint(CRect.Left, CRect.Bottom);

        topLeft = AppliedMoveScaleRotate(topLeft);
        topRight = AppliedMoveScaleRotate(topRight);
        botRight = AppliedMoveScaleRotate(botRight);
        botLeft = AppliedMoveScaleRotate(botLeft);

        var left = CMaths.Min(topLeft.X, topRight.X, botRight.X, botLeft.X);
        var right = CMaths.Max(topLeft.X, topRight.X, botRight.X, botLeft.X);
        var top = CMaths.Min(topLeft.Y, topRight.Y, botRight.Y, botLeft.Y);
        var bottom = CMaths.Max(topLeft.Y, topRight.Y, botRight.Y, botLeft.Y);

        return new CRect(left, top, right - left, bottom - top);
    }

    public CPoint AppliedMoveScaleRotate(CPoint theoryPoint)
    {
        if (Math.Round(Scaling, 4) == 0)
            throw new InvalidOperationException("Scaling cannot be 0.");

        // First move the point.
        theoryPoint.X += Move.Width;
        theoryPoint.Y += Move.Height;

        // Now origin for scaling and rotation.
        theoryPoint.X -= Origin.X;
        theoryPoint.Y -= Origin.Y;

        // Apply scaling
        if (!Scaling.Almost(1))
        {
            theoryPoint.X *= Scaling;
            theoryPoint.Y *= Scaling;
        }

        // Apply rotation
        if (!Rotation.Almost(0))
            if (!theoryPoint.X.Almost(0) || !theoryPoint.Y.Almost(0))
                theoryPoint = theoryPoint.RotatePoint(Rotation);

        // Undo the origin
        theoryPoint.X += Origin.X;
        theoryPoint.Y += Origin.Y;

        return theoryPoint;
    }

    public CPoint ApplyReversed(CPoint theoryPoint)
    {
        if (Math.Round(Scaling, 4) == 0)
            throw new InvalidOperationException("Scaling cannot be 0.");

        // Rotate and Scale relative to origin
        theoryPoint.X -= Origin.X;
        theoryPoint.Y -= Origin.Y;

        // Apply rotation
        if (!theoryPoint.X.Almost(0) || !theoryPoint.Y.Almost(0) && !Rotation.Almost(0))
            theoryPoint = theoryPoint.RotatePoint(-Rotation);

        // Apply scaling
        theoryPoint.X /= Scaling;
        theoryPoint.Y /= Scaling;

        // Undo Rotate and Scale relative to origin
        theoryPoint.X += Origin.X;
        theoryPoint.Y += Origin.Y;

        // Apply shift after
        theoryPoint.X += Move.Width;
        theoryPoint.Y += Move.Height;

        return theoryPoint;
    }

    /// <summary>Returns a new theory with - rotation around origin, 1 / scaling, and - shift.</summary>
    public CTheory Reverse()
    {
        var theory = new CTheory
        {
            Rotation = -this.Rotation,
            Origin = this.Origin,
            Scaling = this.Scaling.Almost(0) ? 0f : (1f / Scaling),
            Move = this.Move.Reverse()
        };
        return theory;
    }
}