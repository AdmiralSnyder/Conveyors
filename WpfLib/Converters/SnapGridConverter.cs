using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfLib.Converters;

public class SnapGridConverter : ConverterBase<SnapGridConverter>, IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 3 &&
            values[0] is double xd &&
            values[1] is double yd &&
            values[2] is int spacing && spacing != 0)
        {
            var x = (int)xd;
            var y = (int)yd;

            x /= spacing;
            y /= spacing;
            x++;
            y++;
            var res = new Point[x * y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    res[i * y + j] = new Point(i * spacing - 0.5, j * spacing - 0.5);
                }
            }
            return res;
            //return Enumerable.Range(1, x).SelectMany(y => Enumerable.Range(1, y).Select(x => new Point(x * 10, y * 10)));
        }
        return Enumerable.Empty<Point>();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
