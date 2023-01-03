using Omega.WpfCommon1;

using System.Drawing;

namespace Omega.WpfApp1;

public partial class OcrPageProfilingControl1 : UserControl
{
    public OcrPageProfilingControl1()
    {
        InitializeComponent();
    }

    public void DoSelectedFileChanged()
    {
        zoom.PanX = 0;
        zoom.PanY = 0;
        zoom.ZoomWidth = 0.45f;
        zoom.ZoomHeight = 0.45f;
    }

    public void DoSelectedPageChanged(Bitmap? bmp)
    {
        if (bmp == null)
            ppc.PageImageSource = null;
        else
            ppc.PageImageSource = BitmapConversion.TryToWpfBitmap(bmp);
    }


}