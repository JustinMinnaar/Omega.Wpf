using Omega.WpfControllers1;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// <summary>
    /// Interaction logic for ProjectExplorerControl1.xaml
    /// </summary>
    public partial class ProjectExplorerControl1 : UserControl
    {
        public ProjectExplorerControl1()
        {
            InitializeComponent();
        }

        MainController main => (MainController)DataContext;

        private void AddNewSolutionButton_Click(object sender, RoutedEventArgs e)
        {
            var tokenSource = new CancellationTokenSource(1000);
            var token = tokenSource.Token;

            Dispatcher.InvokeAsync(main.Explorer.AddNewSolutionAsync, System.Windows.Threading.DispatcherPriority.Normal, token);

            SelectedSolutionNameTextBox.Focus();
        }

        private void AddNewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var tokenSource = new CancellationTokenSource(1000);
            var token = tokenSource.Token;

            Dispatcher.InvokeAsync(main.Explorer.AddNewProjectAsync, System.Windows.Threading.DispatcherPriority.Normal, token);

            SelectedProjectNameTextBox.Focus();
        }
    }
}
