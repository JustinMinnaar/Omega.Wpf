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
        DataContext = main;
    }

}
