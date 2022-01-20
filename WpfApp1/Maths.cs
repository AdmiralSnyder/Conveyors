using System;
using System.Windows;
using System.Windows.Shapes;

namespace WpfApp1;

public static class Maths
{
    public static double Distance(Point p1, Point p2)
    {
        p1.X = p2.X - p1.X;
        p1.X = p1.X * p1.X;
        p1.Y = p2.Y - p1.Y;
        p1.Y = p1.Y * p1.Y;
        return Math.Sqrt(p1.X + p1.Y);
    }

    public static double Length(this Line line) => Distance(new(line.X1, line.Y1), new(line.X2, line.Y2));
}
