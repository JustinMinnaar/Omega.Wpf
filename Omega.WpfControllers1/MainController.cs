using Bdo.DatabaseLibrary1;

using Jem.CommonLibrary22;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Omega.WpfControllers1;

public class MainController : CNotifyPropertyChanged
{
    public MainController()
    {
        Settings = new UserSettingsController(this);
        Explorer = new DocumentsController(this);
        ImportJob = new ImportJobController(this);
        Profiling = new ProfilingController(this);
        ProfilingJob = new ProfilingJobController(this);
    }

    public UserSettingsController Settings { get; set; }
    public DocumentsController Explorer { get; set; }
    public ProfilingController Profiling { get; set; }
    public ImportJobController ImportJob { get; set; }
    public ProfilingJobController ProfilingJob { get; set; }


    public Rect? LastRectangleDrawn;

    public string? LastError { get; set; }
    public bool HasLastError => LastError != null;

    //public event EventHandler LoadCompleted;

    public async Task EnsureDatabaseCreatedAsync()
    {
        using var db = new BdoDocDbContext();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task TryLoadAsync()
    {
        try
        {
            await Settings.LoadAsync();            

            await Explorer.LoadSolutionsAsync();

            Profiling.LoadProfiles();
            //LoadCompleted?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
        }
    }

}
