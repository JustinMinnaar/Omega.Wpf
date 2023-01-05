namespace Omega.WpfApp1;

using Jem.WpfCommonLibrary22;
using Jem.OcrLibrary22.Windows;
using Omega.WpfCommon1;
using Omega.WpfControllers1;

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
    private readonly MainController controller;

    public MainWindow()
    {
        InitializeComponent();

        controller = new MainController();
        controller.Explorer.SelectedSolutionChanged += (sender, e) => Dispatcher.InvokeAsync(controller.Explorer.LoadProjectsAsync);
        controller.Explorer.SelectedProjectChanged += (sender, e) => Dispatcher.InvokeAsync(controller.Explorer.LoadFoldersAsync);
        controller.Explorer.SelectedIdentifiedFilterChanged += (sender, e) => Dispatcher.InvokeAsync(controller.Explorer.LoadFoldersAsync);
        controller.Explorer.SelectedFolderChanged += (sender, e) => Dispatcher.InvokeAsync(controller.Explorer.LoadFilesAsync);
        controller.Explorer.SelectedFileChanged += Explorer_SelectedFileChanged;
        controller.Explorer.SelectedPageChanged += Explorer_SelectedPageChanged;

        // Force initial load of data to happen in separate thread
        Dispatcher.InvokeAsync(LoadAsync);

        DataContext = controller;
    }

    private async Task LoadAsync()
    {
        await controller.TryLoadAsync(Environment.UserName);
    }

    private void Explorer_SelectedFileChanged(object? sender, EventArgs e)
    {
        Dispatcher.InvokeAsync(controller.Explorer.AfterFileChangedAsync);

        this.pageExplorer.DoSelectedFileChanged();
    }

    private void Explorer_SelectedPageChanged(object? sender, EventArgs e)
    {
        controller.Explorer.LoadPage();

        var oPage = controller.Explorer.OPage;
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
        if (controller.Profiling.Profiles == null) return;

        var newProfile = new ProfileModel { Id = Guid.NewGuid(), Name = controller.Explorer.LastRectangleText ?? "(New)" };
        controller.Profiling.Profiles.Add(newProfile);
        controller.Profiling.SelectedProfile = newProfile;
    }
}
