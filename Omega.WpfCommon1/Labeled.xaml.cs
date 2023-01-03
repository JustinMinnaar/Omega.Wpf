using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
using System.Xml.Linq;

namespace Jem.WpfCommonLibrary22
{
    /// <summary>
    ///     Shows a prefix before and suffix after the child control.
    ///     TextBlocks are used to display the prefix and suffix.
    ///     ContentPresenter is used to display the child control, any type of control can be set as the child.
    /// </summary>
    public partial class Labeled : UserControl
    {
        #region Prefix

        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(
            nameof(Prefix), typeof(string), typeof(Labeled), new PropertyMetadata(default(string)));

        public string Prefix
        {
            get => (string)GetValue(PrefixProperty);
            set => SetValue(PrefixProperty, value);
        }

        #endregion

        #region MinimumPrefixWidth


        public static readonly DependencyProperty MinimumPrefixWidthProperty = DependencyProperty.Register(
            nameof(MinimumPrefixWidth), typeof(double), typeof(Labeled), new PropertyMetadata(default(double)));

        public double MinimumPrefixWidth
        {
            get => (double)GetValue(MinimumPrefixWidthProperty);
            set => SetValue(MinimumPrefixWidthProperty, value);
        }

        #endregion

        #region Suffix

        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(
            nameof(Suffix), typeof(string), typeof(Labeled), new PropertyMetadata(default(string)));

        public string Suffix
        {
            get => (string)GetValue(SuffixProperty);
            set => SetValue(SuffixProperty, value);
        }

        #endregion

        #region MinimumSuffixWidth


        public static readonly DependencyProperty MinimumSuffixWidthProperty = DependencyProperty.Register(
            nameof(MinimumSuffixWidth), typeof(double), typeof(Labeled), new PropertyMetadata(default(double)));

        public double MinimumSuffixWidth
        {
            get => (double)GetValue(MinimumSuffixWidthProperty);
            set => SetValue(MinimumSuffixWidthProperty, value);
        }

        #endregion

        #region Child

        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register(
            nameof(Child), typeof(object), typeof(Labeled), new PropertyMetadata(default(object)));

        public object Child
        {
            get => GetValue(ChildProperty);
            set => SetValue(ChildProperty, value);
        }

        #endregion

        public Labeled()
        {
            InitializeComponent();
        }
    }
}
