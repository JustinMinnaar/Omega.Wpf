namespace Jem.CommonLibrary22;

public sealed class TraceWriter : AbstractWriter
{
    public override void ClearLine()
    {
        Trace.WriteLine("");
    }

    protected override void DoWrite(string message)
    {
        Trace.Write(message);
    }

    protected override void DoWriteLine(string? message)
    {
        Trace.WriteLine(message);
    }
}
