using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Omega.WpfCommon1;

public class DummyConverter : MarkupExtension, IValueConverter
{
    private static DummyConverter? _converter = null;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _converter ??= new DummyConverter();
    }

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
        return value; // set breakpoint here to debug your binding
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
        return value;
    }

    #endregion
}
