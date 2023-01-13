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
    public partial class ProfilingJobControl1 : UserControl
    {
        public ProfilingJobControl1()
        {
            InitializeComponent();
        }

        MainController Main => (MainController)DataContext;

    }
}
