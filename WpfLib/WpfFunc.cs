using CoreLib;
using PointDef;
using PointDef.twopoints;
using System;
using System.Drawing;
using UILib.Shapes;
using WpfLib.Shapes;

namespace WpfLib;

public static class ShapeFunc 
{
    public static IShapeFunc Instance { get; set; }
    public static void ApplyMouseBehavior(this IShape shape, Action<IShape> behavior, MouseAction mouseAction = MouseAction.LeftClick)
        => Instance.ApplyMouseBehavior(shape, behavior, mouseAction);

}

public interface IShapeFunc
{
    void ApplyMouseBehavior(IShape shape, Action<IShape> behavior, MouseAction mouseAction = MouseAction.LeftClick);
}

public class ShapeFuncInstanceWpf : IShapeFunc
{
    public void ApplyMouseBehavior(IShape shape, Action<IShape> behavior, MouseAction mouseAction = MouseAction.LeftClick)
    => ((WpfShape)shape).BackingShape.InputBindings.Add(new MouseBinding(new MyCommand<IShape>(behavior, shape), new(mouseAction)));
}


public static class WpfFunc
{
    public static V2d AsPoint(this System.Windows.Point point) => (point.X, point.Y);
    public static System.Windows.Point AsPoint(this V2d point) => new(point.X, point.Y);

    public static Shape SetLocationWpf(this Shape shape, Point location)
    {
        Canvas.SetLeft(shape, location.X);
        Canvas.SetTop(shape, location.Y);
        return shape;
    }

    public static Line SetLocationWpf(this Line line, TwoPoints points)
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

    public static V2d GetSizeWpf<TShape>(this TShape shape) where TShape : Shape => (shape.Width, shape.Height);

    public static System.Drawing.Color AsColor(this System.Media.Color)
}
