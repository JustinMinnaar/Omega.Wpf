using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

namespace Omega.WpfProfilingLibrary1;

public class WpfStyling : Control
{
    public static WpfStyling Instance = new();

    #region HeadingFontSize

    public double HeadingFontSize
    {
        get { return (double)GetValue(HeadingFontSizeProperty); }
        set { SetValue(HeadingFontSizeProperty, value); }
    }

    public static readonly DependencyProperty HeadingFontSizeProperty = DependencyProperty.Register
        (nameof(HeadingFontSize), typeof(double), typeof(WpfStyling), new FrameworkPropertyMetadata((double)18f, FrameworkPropertyMetadataOptions.AffectsMeasure));

    #endregion HeadingFontSize

    #region HeadingMargin

    public Thickness HeadingMargin
    {
        get { return (Thickness)GetValue(HeadingMarginProperty); }
        set { SetValue(HeadingMarginProperty, value); }
    }

    public static readonly DependencyProperty HeadingMarginProperty = DependencyProperty.Register
        (nameof(HeadingMargin), typeof(Thickness), typeof(WpfStyling), new FrameworkPropertyMetadata(new Thickness(3), FrameworkPropertyMetadataOptions.AffectsMeasure));

    #endregion HeadingMargin

    #region ShowConfidencePercentages

    public bool ShowConfidencePercentages
    {
        get { return (bool)GetValue(ShowConfidencePercentagesProperty); }
        set { SetValue(ShowConfidencePercentagesProperty, value); }
    }

    public static readonly DependencyProperty ShowConfidencePercentagesProperty = DependencyProperty.Register(nameof(ShowConfidencePercentages),
            typeof(bool), typeof(WpfStyling), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));

    #endregion ShowConfidencePercentages

    #region Spacing

    public Size FieldSpacing
    {
        get { return (Size)GetValue(SpacingProperty); }
        set { SetValue(SpacingProperty, value); }
    }

    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(nameof(FieldSpacing),
        typeof(Size), typeof(WpfStyling), new FrameworkPropertyMetadata(new Size(3, 3), FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    #endregion Spacing

    #region FieldMargin

    public Thickness FieldMargin
    {
        get { return (Thickness)GetValue(FieldMarginProperty); }
        set { SetValue(FieldMarginProperty, value); }
    }

    public static readonly DependencyProperty FieldMarginProperty = DependencyProperty.Register(nameof(FieldMargin),
        typeof(Thickness), typeof(WpfStyling), new FrameworkPropertyMetadata(new Thickness(3), FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    #endregion FieldMargin

    #region FieldCaptionWidth

    public double FieldCaptionWidth
    {
        get { return (double)GetValue(FieldCaptionWidthProperty); }
        set { SetValue(FieldCaptionWidthProperty, value); }
    }

    public static readonly DependencyProperty FieldCaptionWidthProperty = DependencyProperty.Register(nameof(FieldCaptionWidth),
        typeof(double), typeof(WpfStyling), new FrameworkPropertyMetadata((double)80, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    #endregion FieldCaptionWidth

    #region FieldConfidenceWidth

    public double FieldConfidenceWidth
    {
        get { return (double)GetValue(FieldConfidenceWidthProperty); }
        set { SetValue(FieldConfidenceWidthProperty, value); }
    }

    public static readonly DependencyProperty FieldConfidenceWidthProperty = DependencyProperty.Register(nameof(FieldConfidenceWidth),
        typeof(double), typeof(WpfStyling), new FrameworkPropertyMetadata((double)30, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    #endregion FieldConfidenceWidth

    #region ShowFieldNumbers

    public bool ShowFieldNumbers
    {
        get { return (bool)GetValue(ShowFieldNumbersProperty); }
        set { SetValue(ShowFieldNumbersProperty, value); }
    }

    public static readonly DependencyProperty ShowFieldNumbersProperty = DependencyProperty.Register
        (nameof(ShowFieldNumbers), typeof(bool), typeof(WpfStyling), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));

    #endregion ShowFieldNumbers

    #region Validation

    public double ValidationLineHeight = 1;
    public SolidColorBrush ValidationPassedBrush = Brushes.DarkGreen;
    public SolidColorBrush ValidationFailedBrush = Brushes.DarkRed;

    #endregion Validation

    #region Fields

    public CultureInfo Culture = CultureInfo.CurrentCulture;

    public Typeface FieldCaptionTypeface = new Typeface("Arial");
    public double FieldCaptionSize = 12;
    public Color FieldCaptionColor = Colors.Black;

    public Typeface FieldConfidenceTypeface = new Typeface("Courier New");
    public double FieldConfidenceSize = 6;
    public Color FieldConfidenceColor = Colors.Black;

    public FormattedText? TryGetFieldCaptionFormattedText(string text, double pixelsPerDip)
    {
        return TryGetFormattedText(text, FieldCaptionTypeface, FieldCaptionSize, FieldCaptionColor, pixelsPerDip);
    }

    public FormattedText? TryGetFieldConfidenceFormattedText(decimal? confidence, double pixelsPerDip)
    {
        if (confidence == null) return null;
        return TryGetFormattedText(confidence.Value.ToString("0") + "%", FieldConfidenceTypeface, FieldConfidenceSize, FieldConfidenceColor, pixelsPerDip);
    }

    public FormattedText? TryGetFormattedText(string text, Typeface typeFace, double size, Color color, double pixelsPerDip)
    {
        if (text == null) return null;

        var brush = new SolidColorBrush(color);
        return new FormattedText(text, Culture, FlowDirection, typeFace, FieldCaptionSize, brush, pixelsPerDip);
    }

    public static (Brush outBrush, Pen outPen) GeneratePenAndBrush(Color goodColor)
    {
        var fillColor = Color.FromArgb(50, goodColor.R, goodColor.G, goodColor.B);
        var borderColor = Color.FromArgb(200, goodColor.R, goodColor.G, goodColor.B);

        var outPen = new Pen(new SolidColorBrush(borderColor), 1);
        var outBrush = new SolidColorBrush(fillColor);

        // Speed up performance by reducing quality
        RenderOptions.SetCachingHint(outBrush, CachingHint.Cache);
        RenderOptions.SetBitmapScalingMode(outBrush, BitmapScalingMode.LowQuality);

        return (outBrush, outPen);
    }

    public static void GeneratePenAndBrush(Color goodColor, out Brush outBrush, out Pen outPen)
    {
        var fillColor = Color.FromArgb(50, goodColor.R, goodColor.G, goodColor.B);
        var borderColor = Color.FromArgb(200, goodColor.R, goodColor.G, goodColor.B);

        outPen = new Pen(new SolidColorBrush(borderColor), 1);
        outBrush = new SolidColorBrush(fillColor);

        // Speed up performance by reducing quality
        RenderOptions.SetCachingHint(outBrush, CachingHint.Cache);
        RenderOptions.SetBitmapScalingMode(outBrush, BitmapScalingMode.LowQuality);
    }

    #endregion Fields
}
