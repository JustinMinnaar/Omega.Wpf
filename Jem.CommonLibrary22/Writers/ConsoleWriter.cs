namespace Jem.CommonLibrary22;

public sealed class ConsoleWriter : AbstractWriter
{
    private readonly string blankLine;

    public ConsoleWriter()
    {
        blankLine = new string(' ', Console.WindowWidth - 1);
    }

    public override void ClearLine()
    {
        Console.CursorLeft = 0;
        Console.Write(blankLine);
        Console.CursorLeft = 0;
    }

    protected override void DoWrite(string message)
    {
        ClearLine();
        if (message.Length > blankLine.Length)
            Console.Write(message[..blankLine.Length]);
        else
            Console.Write(message);
    }

    protected override void DoWriteLine(string? message)
    {
        Console.CursorLeft = 0;
        Console.WriteLine(message);
    }
}
