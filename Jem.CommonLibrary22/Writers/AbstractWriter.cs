namespace Jem.CommonLibrary22;

public abstract class AbstractWriter : IWriter
{
    readonly object padlock = new();
    DateTime lastWriteTime = DateTime.Now;

    public abstract void ClearLine();

    public void Write(string message)
    {
        if (DateTime.Now.Subtract(lastWriteTime).TotalMilliseconds > 1000)
        {
            lock (padlock)
            {
                lastWriteTime = DateTime.Now;
                DoWrite(message);
            }
        }
    }


    public void WriteLine(string? message = null)
    {
        lock (padlock)
        {
            DoWriteLine(message);
        }
    }

    public void WriteLine(Exception? ex)
    {
        while (ex != null)
        {
            WriteLine(ex.Message);
            ex = ex.InnerException;
        }
    }

    protected abstract void DoWriteLine(string? message);
    protected abstract void DoWrite(string message);
}
