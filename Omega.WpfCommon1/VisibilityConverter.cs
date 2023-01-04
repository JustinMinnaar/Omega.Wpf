using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Omega.WpfCommon1;

public class VisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool booleanValue = (bool)value;

        if (booleanValue)
        {
            return Visibility.Visible;
        }
        else
        {
            return Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Visibility visibility = (Visibility)value;

        if (visibility == Visibility.Visible)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
