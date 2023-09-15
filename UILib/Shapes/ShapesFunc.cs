using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UILib.Shapes;

public static class ShapesFunc
{
    public static void SetEnd(this ILine line, Point point)
    {
        line.X2 = point.X;
        line.Y2 = point.Y;
    }

    public static void SetStart(this ILine line, Point point)
    {
        line.X1 = point.X;
        line.Y1 = point.Y;
    }
}
