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
        main.Explorer.SelectedFileChanged += Explorer_SelectedFileChanged;
        main.Explorer.SelectedPageChanged += Explorer_SelectedPageChanged;
        main.Settings.PropertyChanged += Settings_PropertyChanged;

    }

    private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Explorer_SelectedPageChanged(sender, e);
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
