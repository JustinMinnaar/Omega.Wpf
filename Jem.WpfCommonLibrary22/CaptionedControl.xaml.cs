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
    /// In this version of the CaptionControl, the TextBlock is used to display the caption, and the ContentPresenter is used to display the child control. The Content property of the ContentPresenter is bound to the Child property of the CaptionControl, which allows any type of control to be set as the child.
    /// the ContentPresenter is used to display the content of a ContentControl, which is bound to the Child property of the control. This allows the CaptionControl to be used with the desired XAML format, where the child control is included as a child element of the CaptionControl.
    /// To use the CaptionControl with this format, you can include the child control as a child element of the CaptionControl in the XAML code.
    /// </summary>
    public partial class CaptionedControl : UserControl
    {
        #region Caption

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            nameof(Caption), typeof(string), typeof(CaptionedControl), new PropertyMetadata(default(string)));

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        #endregion

        #region MinimumCaptionWidth


        public static readonly DependencyProperty MinimumCaptionWidthProperty = DependencyProperty.Register(
            nameof(MinimumCaptionWidth), typeof(double), typeof(CaptionedControl), new PropertyMetadata(default(double)));

        public double MinimumCaptionWidth
        {
            get => (double)GetValue(MinimumCaptionWidthProperty);
            set => SetValue(MinimumCaptionWidthProperty, value);
        }

        #endregion

        #region Suffix

        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(
            nameof(Suffix), typeof(string), typeof(CaptionedControl), new PropertyMetadata(default(string)));

        public string Suffix
        {
            get => (string)GetValue(SuffixProperty);
            set => SetValue(SuffixProperty, value);
        }

        #endregion

        #region MinimumSuffixWidth


        public static readonly DependencyProperty MinimumSuffixWidthProperty = DependencyProperty.Register(
            nameof(MinimumSuffixWidth), typeof(double), typeof(CaptionedControl), new PropertyMetadata(default(double)));

        public double MinimumSuffixWidth
        {
            get => (double)GetValue(MinimumSuffixWidthProperty);
            set => SetValue(MinimumSuffixWidthProperty, value);
        }

        #endregion

        #region Child

        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register(
            nameof(Child), typeof(object), typeof(CaptionedControl), new PropertyMetadata(default(object)));

        public object Child
        {
            get => GetValue(ChildProperty);
            set => SetValue(ChildProperty, value);
        }

        #endregion

        public CaptionedControl()
        {
            InitializeComponent();
        }
    }
}
