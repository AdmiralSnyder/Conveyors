using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfLib.Converters;

public class MultiConverter : ConverterBase<MultiConverter>, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return System.Convert.ChangeType(value, targetType, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Convert(value, targetType, parameter, culture);
}
