namespace Omega.WpfApp1;

using Jem.WpfCommonLibrary22;
using Jem.OcrLibrary22.Windows;
using Omega.WpfCommon1;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    private readonly ExplorerModel explorer;

    public MainWindow()
    {
        InitializeComponent();

        explorer = new() { Id = Guid.NewGuid(), Name = "Explorer" };
        explorer.SelectedSolutionChanged += (sender, e) => Dispatcher.InvokeAsync(explorer.LoadProjectsAsync);
        explorer.SelectedProjectChanged+= (sender, e) => Dispatcher.InvokeAsync(explorer.LoadFolders);
        explorer.SelectedIdentifiedFilterChanged += (sender, e) => Dispatcher.InvokeAsync(explorer.LoadFolders);
        explorer.SelectedFolderChanged += (sender, e) => Dispatcher.InvokeAsync(explorer.LoadFiles);
        explorer.SelectedFileChanged += Explorer_SelectedFileChanged;//(sender, e) => Dispatcher.InvokeAsync(explorer.AfterFileChanged);
        explorer.SelectedPageChanged += Explorer_SelectedPageChanged;// (sender, e)  => Dispatcher.Invoke(explorer.LoadPage);

        // Force initial load of data to happen in separate thread
        Dispatcher.InvokeAsync(explorer.LoadRootsAsync);

        DataContext = explorer;
    }

    private void Explorer_SelectedFileChanged(object? sender, EventArgs e)
    {
        Dispatcher.InvokeAsync(explorer.AfterFileChanged);

        pageExplorer.DoSelectedFileChanged();
    }

    private void Explorer_SelectedPageChanged(object? sender, EventArgs e)
    {        
        explorer.LoadPage();

        var oPage = explorer.oPage;
        if (oPage == null)
        {
            pageExplorer.DoSelectedPageChanged(null);
        }
        else
        {
            var bmp = oPage.ToBitmap(inverse: true);
            pageExplorer.DoSelectedPageChanged(bmp);
        }
    }

    private void AddProfileButton_Click(object sender, RoutedEventArgs e)
    {
        if (explorer.Profiles == null) return;

        var newProfile = new ProfileModel {Id = Guid.NewGuid(), Name = explorer.LastRectangleText ?? "(New)" };
        explorer.Profiles.Add(newProfile);
        explorer.SelectedProfile = newProfile;
    }
}
