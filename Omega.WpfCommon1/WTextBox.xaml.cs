using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Omega.WpfCommon1;

/// <summary>A caption followed by a textbox.</summary>
public partial class WTextBox : UserControl
{
    public WTextBox()
    {
        InitializeComponent();
    }

    #region Prefix

    [Bindable(true)]
    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public string? Prefix
    {
        get { return (string?)GetValue(PrefixProperty); }
        set { SetValue(PrefixProperty, value); }
    }

    public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(
        nameof(Prefix), typeof(string), typeof(WTextBox), new PropertyMetadata(null));

    #endregion

    #region MinPrefixWidth

    public int? MinPrefixWidth
    {
        get { return (int?)GetValue(MinPrefixWidthProperty); }
        set { SetValue(MinPrefixWidthProperty, value); }
    }

    public static readonly DependencyProperty MinPrefixWidthProperty = DependencyProperty.Register(
        nameof(MinPrefixWidth), typeof(int), typeof(WTextBox), new PropertyMetadata(50));

    #endregion

    #region Suffix

    public string? Suffix
    {
        get { return (string?)GetValue(SuffixProperty); }
        set { SetValue(SuffixProperty, value); }
    }

    public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(
        nameof(Suffix), typeof(string), typeof(WTextBox), new PropertyMetadata(null));

    #endregion

    #region MinSuffixWidth

    public double? MinSuffixWidth
    {
        get { return (double?)GetValue(MinSuffixWidthProperty); }
        set { SetValue(MinSuffixWidthProperty, value); }
    }

    public static readonly DependencyProperty MinSuffixWidthProperty = DependencyProperty.Register(
        nameof(MinSuffixWidth), typeof(double), typeof(WTextBox), new PropertyMetadata(null));

    #endregion

    #region Text

    [Bindable(true)]
    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public string? Text
    {
        get { return (string?)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(WTextBox), new PropertyMetadata(null));

    #endregion

    #region MinTextWidth

    public double? MinTextWidth
    {
        get { return (double?)GetValue(MinTextWidthProperty); }
        set { SetValue(MinTextWidthProperty, value); }
    }

    public static readonly DependencyProperty MinTextWidthProperty = DependencyProperty.Register(
        nameof(MinTextWidth), typeof(double), typeof(WTextBox), new PropertyMetadata(null));

    #endregion
}
