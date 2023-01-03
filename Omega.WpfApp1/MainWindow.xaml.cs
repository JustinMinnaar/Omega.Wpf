namespace Omega.WpfApp1;

using Jem.WpfCommonLibrary22;
using Jem.OcrLibrary22.Windows;
using Omega.WpfCommon1;

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

        zoom.PanX = 0;
        zoom.PanY = 0;
        zoom.ZoomWidth = 0.45f;
        zoom.ZoomHeight = 0.45f;
    }

    private void Explorer_SelectedPageChanged(object? sender, EventArgs e)
    {
        ppc.PageImageSource = null;

        explorer.LoadPage();

        var oPage = explorer.oPage; if (oPage == null) return;

        var bmp = oPage.ToBitmap(inverse:true);
        
        ppc.PageImageSource = BitmapConversion.TryToWpfBitmap(bmp);
    }

    private void AddProfileButton_Click(object sender, RoutedEventArgs e)
    {
        if (explorer.Profiles == null) return;

        var newProfile = new ProfileModel {Id = Guid.NewGuid(), Name = "(NEW)" };
        explorer.Profiles.Add(newProfile);
        explorer.SelectedProfile = newProfile;
    }

    private void zoom_ZoomChanged(object sender, EventArgs e)
    {
        var zoomWidth = zoom.ZoomWidth; var zoomHeight = zoom.ZoomHeight;
    }
}
