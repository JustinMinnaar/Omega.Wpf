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
        explorer.SelectedRootChanged += (sender, e) => Dispatcher.InvokeAsync(explorer.LoadProjectsAsync);
        explorer.SelectedProjectChanged+= (sender, e) => Dispatcher.InvokeAsync(explorer.LoadFolders);
        explorer.SelectedFolderChanged+= (sender, e) => Dispatcher.InvokeAsync(explorer.LoadFiles);
        explorer.SelectedFileChanged += (sender, e) => Dispatcher.InvokeAsync(explorer.AfterFileChanged);
        explorer.SelectedPageChanged += Explorer_SelectedPageChanged;// (sender, e)  => Dispatcher.Invoke(explorer.LoadPage);

        // Force initial load of data to happen in separate thread
        Dispatcher.InvokeAsync(explorer.LoadRootsAsync);

        DataContext = explorer;
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
}
