using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ConveyorApp.Converters;

public class ConverterBase<TConverter> : MarkupExtension
    where TConverter: ConverterBase<TConverter>, new()
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}


public class MultiConverter : ConverterBase<MultiConverter >, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return System.Convert.ChangeType(value, targetType, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        => Convert(value, targetType, parameter, culture);
}

internal class WidthToRectConverter : ConverterBase<WidthToRectConverter>, IValueConverter
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