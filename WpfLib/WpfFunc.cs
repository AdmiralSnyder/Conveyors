using System;

namespace WpfLib;

public static class WpfFunc
{
    public static void ApplyMouseBehaviour(this Shape shape, Action<Shape> behaviour, MouseAction mouseAction = MouseAction.LeftClick) => shape.InputBindings.Add(new MouseBinding(new MyCommand<Shape>(behaviour, shape), new(mouseAction)));

    public static void SetLocation(this Shape shape, Point location)
    {
        Canvas.SetLeft(shape, location.X);
        Canvas.SetTop(shape, location.Y);
    }

    public static void SetLocation(this Line line, TwoPoints points)
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

    }

    public static void SetCenterLocation(this Shape shape, Point location) => shape.SetLocation(location.Subtract((shape.Width / 2, shape.Height / 2)));

}
