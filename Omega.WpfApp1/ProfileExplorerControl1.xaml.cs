using Omega.WpfControllers1;

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
    /// <summary>
    /// Interaction logic for ProfileExplorerControl1.xaml
    /// </summary>
    public partial class ProfileExplorerControl1 : UserControl
    {
        public ProfileExplorerControl1()
        {
            InitializeComponent();
        }

        MainController main => (MainController)DataContext;

        private bool AskAddItem(string itemType)
        {
            var name = main.Explorer.LastRectangleText ?? "(New)";

            var text = $"Do you want to add {itemType} '{name}'?";
            var caption = "Add " + itemType;

            var yn = MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            return (yn == MessageBoxResult.Yes);
        }

        private void AddBagButton_Click(object sender, RoutedEventArgs e)
        {
            if (AskAddItem("bag"))
                main.Profiling.AddBag();
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (AskAddItem("group"))
                main.Profiling.AddGroup();
        }

        private void AddProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (AskAddItem("profile"))
                main.Profiling.AddProfile();
        }

        private void AddTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            if (AskAddItem("template"))
                main.Profiling.AddTemplate();
        }
    }
}
