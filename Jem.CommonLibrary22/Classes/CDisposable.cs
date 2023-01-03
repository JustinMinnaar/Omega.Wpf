namespace Jem.CommonLibrary22;

/// <summary>This is a class that is disposable. The inheritor should override the Dispose(bool) method.</summary>
public class CDisposable : IDisposable
{
    public CDisposable()
    {
    }

    public CDisposable(CDisposable other) : base()
    {
    }

    /// <summary>Should be overwritten by the inheriting class.</summary>
    /// <param name="disposing">True if Dispose was called, false otherwise.</param>
    /// <remarks>Overwrite this method and clean up any resources that are not null, setting their reference to null afterwards.</remarks>
    protected virtual void Dispose(bool disposing) { }

    /// <summary>Do not change this code. Override <see cref="Dispose(bool)"/>.</summary>
    public void Dispose()
    {
        Dispose(true);

        // TODO: Determine if this prevent pix objects from cleaning up memory
        //    GC.SuppressFinalize(this);
    }
}