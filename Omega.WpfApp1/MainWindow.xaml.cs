using Omega.WpfModels1;

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

namespace Omega.WpfApp1
{
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
            explorer.SelectedFileChanged += (sender, e) => Dispatcher.InvokeAsync(explorer.LoadPages);

            Dispatcher.InvokeAsync(explorer.LoadRootsAsync);

            DataContext = explorer;
        }

        private void AddProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (explorer.Profiles == null) return;

            var newProfile = new ProfileModel {Id = Guid.NewGuid(), Name = "(NEW)" };
            explorer.Profiles.Add(newProfile);
            explorer.SelectedProfile = newProfile;
        }
    }
}
