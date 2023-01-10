using System;
using System.Collections;
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

public partial class WComboBox : UserControl
{
    public WComboBox()
    {
        InitializeComponent();
    }

    #region Prefix

    public string? Prefix
    {
        get { return (string?)GetValue(PrefixProperty); }
        set { SetValue(PrefixProperty, value); }
    }

    public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(
        nameof(Prefix), typeof(string), typeof(WComboBox), new PropertyMetadata(null));

    #endregion

    #region Buttons

    public string? Buttons
    {
        get { return (string?)GetValue(ButtonsProperty); }
        set { SetValue(ButtonsProperty, value); }
    }

    public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(
        nameof(Buttons), typeof(string), typeof(WComboBox), new PropertyMetadata(null));

    #endregion

    #region MinPrefixWidth

    public int? MinPrefixWidth
    {
        get { return (int?)GetValue(MinPrefixWidthProperty); }
        set { SetValue(MinPrefixWidthProperty, value); }
    }

    public static readonly DependencyProperty MinPrefixWidthProperty = DependencyProperty.Register(
        nameof(MinPrefixWidth), typeof(int), typeof(WComboBox), new PropertyMetadata(50));

    #endregion

    #region Suffix

    public string? Suffix
    {
        get { return (string?)GetValue(SuffixProperty); }
        set { SetValue(SuffixProperty, value); }
    }

    public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(
        nameof(Suffix), typeof(string), typeof(WComboBox), new PropertyMetadata(null));

    #endregion

    #region Items

    [Bindable(true)]
    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public IEnumerable ItemsSource
    {
        get { return (IEnumerable)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource), typeof(IEnumerable), typeof(WComboBox), new PropertyMetadata(null));

    #endregion

    #region SelectedValue

    [Bindable(true)]
    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public object? SelectedValue
    {
        get { return (object?)GetValue(SelectedValueProperty); }
        set { SetValue(SelectedValueProperty, value); }
    }

    public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(
        nameof(SelectedValue), typeof(object), typeof(WComboBox), new PropertyMetadata(null));

    #endregion

    #region SelectedValuePath

    public string? SelectedValuePath
    {
        get { return (string?)GetValue(SelectedValuePathProperty); }
        set { SetValue(SelectedValuePathProperty, value); }
    }

    public static readonly DependencyProperty SelectedValuePathProperty = DependencyProperty.Register(
        nameof(SelectedValuePath), typeof(string), typeof(WComboBox), new PropertyMetadata(null));

    #endregion

    #region DisplayMemberPath

    public string? DisplayMemberPath
    {
        get { return (string?)GetValue(DisplayMemberPathProperty); }
        set { SetValue(DisplayMemberPathProperty, value); }
    }

    public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register(
        nameof(DisplayMemberPath), typeof(string), typeof(WComboBox), new PropertyMetadata(null));

    #endregion

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectionChanged?.Invoke(this, e);
    }
}
