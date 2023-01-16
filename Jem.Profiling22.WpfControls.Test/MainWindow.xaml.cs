using Jem.OcrLibrary22;
using Jem.OcrLibrary22.Windows;
using Jem.WpfCommonLibrary22;

using System;
using System.Collections.Generic;
using System.IO;
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

namespace Jem.Profiling22.WpfControls.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected async override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var folder = @"F:\BDO\E-Lifestyle Uploads\00a1f211-7c81-4f78-b3c8-3-2078\";
            var file = @"3a8711c9-ca16-4268-8b62-e992b44bb05a_00a1f211-7c81-4f78-b3c8-3-2078_October2021";
            var ocrPath = folder + file + ".bocr";
            var oDocument = await OcrDocument.TryLoadFromBinaryFileAsync(ocrPath);
            if (oDocument != null)
            {
                oDocument.ResizeTo2100();
                var p0 = oDocument.Pages[0];
                p0.SaveBitmap(oDocument, overwrite: true, startPaintDotNet: true);
                
                var bmp = p0.ToBitmap();
                var image = bmp.ToWpfBitmap();
                ppc.PageImageSource = image;
            }
            else
            {
                //var pngPath = folder + file + ".pdf.1.debug.png";
                //var image = await pngPath.AsFilePathToWpfBitmapAsync();
                //ppc.PageImageSource = image;
            }
        }


    }
}
