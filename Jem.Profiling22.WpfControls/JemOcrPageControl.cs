using Jem.OcrLibrary22;
using Jem.OcrLibrary22.Windows;
using Jem.WpfCommonLibrary22;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Jem.Profiling22.WpfControls;

public class JemOcrPageControl : UserControl
{
    #region JemOcrPageControl 

    static JemOcrPageControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(JemOcrPageControl), new FrameworkPropertyMetadata(typeof(JemOcrPageControl)));
    }

    public JemOcrPageControl()
    {
    }

    #endregion JemProfilePageControl

    #region OcrPage

    public OcrPage OcrPage
    {
        get { return (OcrPage)GetValue(OcrPageProperty); }
        set { SetValue(OcrPageProperty, value); }
    }

    public static readonly DependencyProperty OcrPageProperty = DependencyProperty.Register
        (nameof(OcrPage), typeof(OcrPage), typeof(JemProfilePageControl),
         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion OcrPage

    protected override Size MeasureOverride(Size constraint)
    {
        var oPage = OcrPage;
        if (oPage == null) return new Size();

        return new(oPage.Size.Width, oPage.Size.Height);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var bmp = OcrPage?.ToBitmap().ToWpfBitmap();
        if (bmp == null) return;

        drawingContext.DrawImage(bmp, new Rect(0, 0, bmp.Width, bmp.Height));
    }
}
