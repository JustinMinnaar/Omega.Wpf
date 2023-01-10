using Omega.WpfCommon1;
using Omega.WpfControllers1;
using Omega.WpfProfilingLibrary1;

using System.Drawing;

using static Omega.WpfProfilingLibrary1.JemProfilePageControl;

namespace Omega.WpfApp1;

public partial class OcrPageProfilingControl1 : UserControl
{
    public OcrPageProfilingControl1()
    {
        InitializeComponent();
    }

    public MainController Controller => (MainController)DataContext;


    public void DoSelectedFileChanged()
    {
        if (Controller.Settings.ResetPanZoomOnFileSelect != true) return;

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

    private void Ppc_RectangleDrawn(JemProfilePageControl sender, Rect mouseRect)
    {
        var ocrRect = Controller.Explorer.RectangleDrawn(mouseRect);

        if (ocrRect == null)
            ppc.LastOcrRect = Rect.Empty;
        else
            ppc.LastOcrRect = ocrRect.Value.ToRect();

        RectangleDrawn?.Invoke(sender, mouseRect);
    }

    public event MouseRectDelegate? RectangleDrawn, LineDrawn;

    private void ppc_LineDrawn(JemProfilePageControl sender, Rect mouseRect)
    {
        LineDrawn?.Invoke(sender, mouseRect);
    }
}