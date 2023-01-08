using Bdo.DatabaseLibrary1;

using Jem.CommonLibrary22;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Omega.WpfControllers1;

public class MainController : CNotifyPropertyChanged
{
    public MainController()
    {
        Settings = new UserSettingsController(this);

        Explorer = new DocumentsController { Main = this };

        Profiling = new ProfilingController(this);
    }

    public UserSettingsController Settings { get; set; }
    public DocumentsController Explorer { get; set; }
    public ProfilingController Profiling { get; set; }

    public string? LastError { get; set; }
    public bool HasLastError => LastError != null;

    public async Task TryLoadAsync(string userName)
    {
        try
        {
            await Explorer.LoadSolutionsAsync();
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
        }
    }

}
