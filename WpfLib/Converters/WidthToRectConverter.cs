using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfLib.Converters;

public class WidthToRectConverter : ConverterBase<WidthToRectConverter>, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double width = (double)value;
        return new Rect(X, Y, width, width);
    }

    public double X { get; set; }
    public double Y { get; set; }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}