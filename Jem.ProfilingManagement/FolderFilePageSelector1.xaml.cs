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

namespace Jem.ProfilingManagement
{
    /// <summary>
    /// Interaction logic for FolderFilePageSelector1.xaml
    /// </summary>
    public partial class FolderFilePageSelector1 : UserControl
    {
        public FolderFilePageSelector1()
        {
            InitializeComponent();
        }

        #region SplitWidth

        public double? SplitWidth
        {
            get { return (double?)GetValue(SplitWidthProperty); }
            set { SetValue(SplitWidthProperty, value); }
        }

        public static readonly DependencyProperty SplitWidthProperty = DependencyProperty.Register(
            nameof(SplitWidth), typeof(double?), typeof(FolderFilePageSelector1), new PropertyMetadata(null));

        #endregion

    }
}
