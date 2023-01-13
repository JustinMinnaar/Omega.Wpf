using Jem.OcrLibrary22.Windows;

using Omega.WpfControllers1;

using System;
using System.Threading.Tasks;
using System.Windows;

namespace Omega.WpfApp1;

public partial class MainControl1
{

    public MainControl1()
    {
        InitializeComponent();
        DataContextChanged += MainControl1_DataContextChanged;
    }

    private MainController main = default!;

    private void MainControl1_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        main = (MainController)DataContext;
        main.Explorer.SelectedSolutionChanged += (sender, e) => Dispatcher.InvokeAsync(main.Explorer.LoadProjectsAsync);
        main.Explorer.SelectedProjectChanged += (sender, e) => Dispatcher.InvokeAsync(main.Explorer.LoadFoldersAsync);
        main.Explorer.SelectedIdentifiedFilterChanged += (sender, e) => Dispatcher.InvokeAsync(main.Explorer.LoadFoldersAsync);
        main.Explorer.SelectedFolderChanged += (sender, e) => Dispatcher.InvokeAsync(main.Explorer.LoadFilesAsync);
        main.Explorer.SelectedFileChanged += Explorer_SelectedFileChanged;
        main.Explorer.SelectedPageChanged += Explorer_SelectedPageChanged;
        main.Settings.PropertyChanged += Settings_PropertyChanged;

        // Force initial load of data to happen in separate thread
        Dispatcher.InvokeAsync(LoadAsync);
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
        main.Explorer.LastRectangleDrawn = new(mouseRect.X, mouseRect.Y, mouseRect.Width, mouseRect.Height);
    }

}
