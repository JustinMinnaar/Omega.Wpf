namespace Omega.WpfApp1;

using Jem.WpfCommonLibrary22;
using Jem.OcrLibrary22.Windows;
using Omega.WpfCommon1;
using Omega.WpfControllers1;
using Omega.WpfModels1.Profiling;
using Bdo.DatabaseLibrary1.Migrations;

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
        main.Explorer.SelectedFileChanged += Explorer_SelectedFileChanged;
        main.Explorer.SelectedPageChanged += Explorer_SelectedPageChanged;


        main.Settings.PropertyChanged += Settings_PropertyChanged;

        // Force initial load of data to happen in separate thread
        Dispatcher.InvokeAsync(LoadAsync);

        DataContext = main;
    }

    private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Explorer_SelectedPageChanged(sender, e);
    }

    private async Task LoadAsync()
    {
        await main.TryLoadAsync(Environment.UserName);
    }

    private void Explorer_SelectedFileChanged(object? sender, EventArgs e)
    {
        Dispatcher.InvokeAsync(main.Explorer.AfterFileChangedAsync);

        this.pageExplorer.DoSelectedFileChanged();
    }

    private void Explorer_SelectedPageChanged(object? sender, EventArgs e)
    {
        main.Explorer.LoadPage();

        var oPage = main.Explorer.OPage;
        if (oPage == null)
        {
            pageExplorer.DoSelectedPageChanged(null);
        }
        else
        {
            var bmp = oPage.ToBitmap(inverse: main.Settings.DarkMode);
            pageExplorer.DoSelectedPageChanged(bmp);
        }
    }

    private void PageExplorer_RectangleDrawn(WpfProfilingLibrary1.JemProfilePageControl _, Rect mouseRect)
    {
        main.Explorer.LastRectangleDrawn= new(  mouseRect.X, mouseRect.Y, mouseRect.Width, mouseRect.Height);
    }

}
