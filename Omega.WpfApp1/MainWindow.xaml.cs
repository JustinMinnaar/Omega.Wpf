namespace Omega.WpfApp1;

using Jem.WpfCommonLibrary22;
using Jem.OcrLibrary22.Windows;
using Omega.WpfCommon1;
using Omega.WpfControllers1;
using Omega.WpfModels1.Profiling;
using Bdo.DatabaseLibrary1.Migrations;
using System.ComponentModel;

// todo: store the last selected root, project, folder, file, page in the database
// todo: store the last selected profile, template, identifier, etc. in the database
// todo: add a new profile
// todo: add a new template
// todo: add a new identifier
// todo: add a new extractor
// todo: test pages using profiles
// todo: show page not matched

public partial class MainWindow
{
    private readonly MainController main;

    public MainWindow()
    {
        InitializeComponent();

        main = new MainController();
        main.Explorer.SelectedSolutionChanged += (sender, e) => Dispatcher.InvokeAsync(main.Explorer.LoadProjectsAsync);
        main.Explorer.SelectedProjectChanged += (sender, e) => Dispatcher.InvokeAsync(main.Explorer.LoadFoldersAsync);
        main.Explorer.SelectedIdentifiedFilterChanged += (sender, e) => Dispatcher.InvokeAsync(main.Explorer.LoadFoldersAsync);
        main.Explorer.SelectedFolderChanged += (sender, e) => Dispatcher.InvokeAsync(main.Explorer.LoadFilesAsync);
        //main.LoadCompleted += (sender, e) =>
        //{
        //    main.Settings.HasChanged = false;
        //    DataContext = main;
        //};
        // Force initial load of data to happen in separate thread
        Dispatcher.InvokeAsync(LoadAsync);
    }

    private async Task LoadAsync()
    {
        await main.TryLoadAsync(Environment.UserName);
        
        DataContext = main;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        if (main.Settings.HasChanged)
        {
            var t = Task.Run(main.Settings.SaveAsync);
            t.Wait();
        }
    }

}
