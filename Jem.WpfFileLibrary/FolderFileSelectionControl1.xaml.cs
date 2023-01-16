using Jem.FileManager22;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jem.WpfFileLibrary;

public partial class FolderFileSelectionControl1 : UserControl
{
    public FolderFileSelectionControl1()
    {
        InitializeComponent();
    }

    public event EventHandler? SelectedFolderChanged, SelectedFileChanged, SelectedPageChanged;
    // public event EventHandler? SavePropertiesClicked;

    private void lstFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedFolderChanged?.Invoke(sender, e);
        SelectedFileChanged?.Invoke(sender, e);
        SelectedPageChanged?.Invoke(sender, e);
    }

    private void lstFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedFileChanged?.Invoke(sender, e);
        SelectedPageChanged?.Invoke(sender, e);
    }

    private void lstPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedPageChanged?.Invoke(sender, e);
    }
}
