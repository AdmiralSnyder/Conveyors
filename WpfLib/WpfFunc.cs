using CoreLib;
using System;

namespace WpfLib;

public static class WpfFunc
{
    public static void ApplyMouseBehavior(this Shape shape, Action<Shape> behavior, MouseAction mouseAction = MouseAction.LeftClick) => shape.InputBindings.Add(new MouseBinding(new MyCommand<Shape>(behavior, shape), new(mouseAction)));

    public static void SetLocation(this Shape shape, Point location)
    {
        Canvas.SetLeft(shape, location.X);
        Canvas.SetTop(shape, location.Y);
    }

    public static Line SetLocation(this Line line, TwoPoints points)
    {
        if (points.P1.X != line.X1)
        {
            line.X1 = points.P1.X;
        }
        if (points.P1.Y != line.Y1)
        {
            line.Y1 = points.P1.Y;
        }
        if (points.P2.X != line.X2)
        {
            line.X2 = points.P2.X;
        }
        if (points.P2.Y != line.Y2)
        {
            line.Y2 = points.P2.Y;
        }

        return line;
    }

    public static TShape SetCenterLocation<TShape>(this TShape shape, Point location)
        where TShape : Shape => shape.Modify(s => s.SetLocation(location.Subtract((s.Width / 2, s.Height / 2))));

}
