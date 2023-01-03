
using Jem.CommonLibrary22;

public abstract class BaseWithWriter
{
    #region class

    // Writes progress and status reports to the console
    protected readonly IWriter? writer;

    [DebuggerStepThrough]
    public BaseWithWriter(IWriter? writer  = null)
    {
        this.writer = writer;
    }

    #endregion
}