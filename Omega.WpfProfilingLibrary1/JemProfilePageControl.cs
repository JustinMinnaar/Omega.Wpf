using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Jem.Profiling22.Data;

namespace Omega.WpfProfilingLibrary1;

public class JemProfilePageControl : UserControl
{
    #region JemProfilePageControl

    static JemProfilePageControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(JemProfilePageControl), new FrameworkPropertyMetadata(typeof(JemProfilePageControl)));
    }

    public JemProfilePageControl()
    {
    }

    #endregion JemProfilePageControl

    #region PageImageSource

    public ImageSource? PageImageSource
    {
        get { return (ImageSource?)GetValue(PageImageSourceProperty); }
        set { SetValue(PageImageSourceProperty, value); }
    }

    public static readonly DependencyProperty PageImageSourceProperty = DependencyProperty.Register
        (nameof(PageImageSource), typeof(ImageSource), typeof(JemProfilePageControl),
         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion PageImageSource

    #region Templates

    public List<ProTemplate> VisibleTemplates
    {
        get { return (List<ProTemplate>)GetValue(TemplatesProperty); }
        set { SetValue(TemplatesProperty, value); }
    }

    /// <summary>Sets the VisibleTemplates from any thread.</summary>
    public void SetVisibleTemplates(List<ProTemplate> value)
    {
        Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback
            (delegate { VisibleTemplates = value; return null; }), null);
    }

    public static readonly DependencyProperty TemplatesProperty = DependencyProperty.Register
        (nameof(VisibleTemplates), typeof(List<ProTemplate>), typeof(JemProfilePageControl),
         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion Templates

    #region SelectedTemplate

    public ProTemplate SelectedTemplate
    {
        get { return (ProTemplate)GetValue(SelectedTemplateProperty); }
        set { SetValue(SelectedTemplateProperty, value); }
    }

    /// <summary>Sets the SelectedTemplate from any thread.</summary>
    public void SetSelectedTemplate(ProTemplate value)
    {
        Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback
            (delegate { SelectedTemplate = value; return null; }), null);
    }

    public static readonly DependencyProperty SelectedTemplateProperty = DependencyProperty.Register
        (nameof(SelectedTemplate), typeof(ProTemplate), typeof(JemProfilePageControl),
         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion SelectedTemplate

    #region ColorProfilePage

    public Color ColorProfilePage
    {
        get { return (Color)GetValue(ColorProfilePageProperty); }
        set { SetValue(ColorProfilePageProperty, value); }
    }

    public static readonly DependencyProperty ColorProfilePageProperty = DependencyProperty.Register
        (nameof(ColorProfilePage), typeof(Color), typeof(JemProfilePageControl),
         new FrameworkPropertyMetadata(Colors.DarkOrange, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion ColorProfilePage

    #region ColorTemplatePage

    public Color ColorTemplatePage
    {
        get { return (Color)GetValue(ColorTemplatePageProperty); }
        set { SetValue(ColorTemplatePageProperty, value); }
    }

    public static readonly DependencyProperty ColorTemplatePageProperty = DependencyProperty.Register
        (nameof(ColorTemplatePage), typeof(Color), typeof(JemProfilePageControl),
         new FrameworkPropertyMetadata(Colors.DarkOrange, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion ColorTemplatePage

    #region ColorTemplateTable

    public Color ColorTemplateTable
    {
        get { return (Color)GetValue(ColorTemplateTableProperty); }
        set { SetValue(ColorTemplateTableProperty, value); }
    }

    public static readonly DependencyProperty ColorTemplateTableProperty = DependencyProperty.Register
        (nameof(ColorTemplateTable), typeof(Color), typeof(JemProfilePageControl),
         new FrameworkPropertyMetadata(Colors.DarkOrchid, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion ColorTemplateTable

    #region ColorTemplateRow

    public Color ColorTemplateRow
    {
        get { return (Color)GetValue(ColorTemplateRowProperty); }
        set { SetValue(ColorTemplateRowProperty, value); }
    }

    public static readonly DependencyProperty ColorTemplateRowProperty = DependencyProperty.Register
        (nameof(ColorTemplateRow), typeof(Color), typeof(JemProfilePageControl),
         new FrameworkPropertyMetadata(Colors.DarkOliveGreen, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion ColorTemplateRow

    #region ShowTemplates

    public bool ShowTemplates
    {
        get { return (bool)GetValue(ShowTemplatesProperty); }
        set { SetValue(ShowTemplatesProperty, value); }
    }

    public static readonly DependencyProperty ShowTemplatesProperty = DependencyProperty.Register
        (nameof(ShowTemplates), typeof(bool), typeof(JemProfilePageControl),
         new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion ShowTemplates

    #region EditorMode

    public EEditorMode EditorMode
    {
        get { return (EEditorMode)GetValue(EditorModeProperty); }
        set { SetValue(EditorModeProperty, value); }
    }

    public static readonly DependencyProperty EditorModeProperty = DependencyProperty.Register
        (nameof(EditorMode), typeof(EEditorMode), typeof(JemProfilePageControl),
         new FrameworkPropertyMetadata(EEditorMode.Rectangle, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion EditorMode

    #region Mouse

    private bool mouseLeftButtonDown;
    private Point mouseBeginPosition;
    private Point mouseMovePosition;
    private bool mouseRectangleCompleted;
    private Rect mouseRect = new Rect();

    public delegate void MouseRectDelegate(JemProfilePageControl sender, Rect mouseRect);

    public event MouseRectDelegate? RectangleDrawn, LineDrawn;

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        mouseBeginPosition = e.GetPosition(this);
        mouseLeftButtonDown = true;
        mouseRectangleCompleted = false;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (mouseLeftButtonDown)
        {
            CalcMousePosition(e.GetPosition(this));
            InvalidateVisual();
        }
    }

    private void CalcMousePosition(Point p)
    {
        if (mouseMovePosition != p)
        {
            mouseMovePosition = p;
            mouseRect = new Rect(
                Math.Min(mouseBeginPosition.X, mouseMovePosition.X),
                Math.Min(mouseBeginPosition.Y, mouseMovePosition.Y),
                Math.Abs((mouseMovePosition.X - mouseBeginPosition.X)),
                Math.Abs((mouseMovePosition.Y - mouseBeginPosition.Y))
                );
        }
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);

        if (mouseLeftButtonDown)
        {
            CalcMousePosition(e.GetPosition(this));
            mouseLeftButtonDown = false;
            mouseRectangleCompleted = true;

            // TODO: Symbols
            //if (EditorMode == EEditorMode.SymbolsAdd || EditorMode == EEditorMode.SymbolsRemove)
            //{
            //    if (SymbolResults != null)
            //    {
            //        foreach (var symbol in SymbolResults)
            //        {
            //            var ocrSymbolRect = new Rect(symbol.Rect.Left, symbol.Rect.Top, symbol.Rect.Width, symbol.Rect.Height);
            //            var touched = mouseRect.Contains(ocrSymbolRect);
            //            if (touched)
            //            {
            //                if (EditorMode == EEditorMode.SymbolsAdd) symbol.IsSelected = true;
            //                if (EditorMode == EEditorMode.SymbolsRemove) symbol.IsSelected = false;
            //            }
            //        }
            //        InvalidateVisual();
            //    }
            //}

            if (EditorMode == EEditorMode.Rectangle)
            {
                RectangleDrawn?.Invoke(this, mouseRect);
            }

            if (EditorMode == EEditorMode.Line)
            {
                LineDrawn?.Invoke(this, mouseRect);
            }

            InvalidateVisual();
        }
    }

    #endregion Mouse

    #region Render

    private double pixelsPerDip;

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        pixelsPerDip = VisualTreeHelper.GetDpi(this).DpiScaleX;

        OnRenderPageImage(drawingContext);
        //if (ShowSymbols && SymbolResults != null)
        //{
        //    OnRenderSymbolResults(drawingContext);
        //}
        if (ShowTemplates)
        {
            OnRenderTemplates(drawingContext);
        }
        //if (ShowExtractors)
        //{
        //    OnRenderExtractors(drawingContext);
        //}
        //if (ShowSteps)
        //{
        //    OnRenderSteps(drawingContext);
        //}
        //if (OcrPage != null)
        //{
        //    if (ShowOcrText || ShowOcrSymbolRectangles || ShowOcrWordRectangles || ShowOcrSentenceRectangles)
        //        OnRenderOcrText(drawingContext);

        //    if (ShowOcrMatches)
        //        OnRenderOcrMatches(drawingContext);
        //}
        OnRenderMouseSelection(drawingContext);

        //drawingContext.Pop();
    }

    private void OnRenderPageImage(DrawingContext drawingContext)
    {
        if (PageImageSource == null) return;
        if (PageImageSource is not BitmapSource bs) return;

        var piWidth = bs.PixelWidth;
        var piHeight = bs.PixelHeight;

        var rect = new Rect(0, 0, piWidth, piHeight);
        drawingContext.DrawImage(PageImageSource, rect);

        drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.White, 1), rect);
    }

    private void OnRenderMouseSelection(DrawingContext drawingContext)
    {
        if (mouseLeftButtonDown || mouseRectangleCompleted)
        {
            var mouseColor = Colors.Red;
            if (mouseRectangleCompleted) mouseColor = Colors.Blue;

            var mouseFillColor = Color.FromArgb(50, mouseColor.R, mouseColor.G, mouseColor.B);
            var mouseBrush = new SolidColorBrush(mouseFillColor);
            var mousePen = new Pen(new SolidColorBrush(mouseColor), 1);

            drawingContext.DrawRectangle(mouseBrush, mousePen, mouseRect);
        }
    }

    private void OnRenderTemplates(DrawingContext drawingContext)
    {
        if (VisibleTemplates == null || VisibleTemplates.Count == 0) return;

        // var styling = WpfStyling.Instance;
        //if (PageImageSource is not BitmapImage s) return;
        //int width = (int)s.PixelWidth;
        //int height = (int)s.PixelHeight;

        var color = Colors.LightBlue;
        WpfStyling.GeneratePenAndBrush(color, out var brush, out var pen);

        var culture = CultureInfo.GetCultureInfo("en-us");
        var typeFace = new Typeface("Verdana");

        foreach (var template in VisibleTemplates)
        {
            if (template.Rect.IsEmpty) continue;

            //if (PageTheory == null) PageTheory = new ProTheory();

            //var topLeft = PageTheory.Apply(stencil.Rect.TopLeft).ToPoint();
            //var topRight = PageTheory.Apply(stencil.Rect.TopRight()).ToPoint();
            //var bottomRight = PageTheory.Apply(stencil.Rect.BotomRight()).ToPoint();
            //var bottomLeft = PageTheory.Apply(stencil.Rect.BotomLeft()).ToPoint();

            //var topLeft = stencil.BorderRect.TopLeft.ToPoint();
            //var topRight = stencil.BorderRect.TopRight().ToPoint();
            //var bottomRight = stencil.BorderRect.BotomRight().ToPoint();
            //var bottomLeft = stencil.BorderRect.BotomLeft().ToPoint();

            //drawingContext.DrawLine(pen, topLeft, topRight);
            //drawingContext.DrawLine(pen, topRight, bottomRight);
            //drawingContext.DrawLine(pen, bottomRight, bottomLeft);
            //drawingContext.DrawLine(pen, bottomLeft, topLeft);

            drawingContext.DrawRectangle(brush, pen, template.Rect.ToRect());

            var t = new FormattedText($"{template.Name}%", culture, flowDirection: FlowDirection.LeftToRight, typeFace, 16, Brushes.Red, 72);
            var displayHeight = ActualHeight;
            drawingContext.DrawText(t, new Point(4, (displayHeight - t.Height) / 2));
        }
    }

    #endregion Render
}
