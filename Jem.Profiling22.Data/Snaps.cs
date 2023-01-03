namespace Jem.Profiling22.Data;

[Flags]
public enum Snaps
{
    None = 0,
    Left = 1,
    Right = 2,
    Top = 4,
    Bottom = 8,

    LeftAlign = Left + Top + Bottom,
    RightAlign = Right + Top + Bottom,
    TopBottom = Top + Bottom,

}
