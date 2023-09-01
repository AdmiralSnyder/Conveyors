using CoreLib;
using PointDef;
using PointDef.twopoints;
using System;
using System.Drawing;

namespace WpfLib;

public static class WpfFunc
{
    public static void ApplyMouseBehavior(this Shape shape, Action<Shape> behavior, MouseAction mouseAction = MouseAction.LeftClick)
        => shape.InputBindings.Add(new MouseBinding(new MyCommand<Shape>(behavior, shape), new(mouseAction)));

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
}

public class UIHelpersInstanceWpf : IUIHelpers
{
    public V2d GetSize<TShape>(TShape shape) => ((Shape)(object)shape).GetSizeWpf();
    public TShape SetLocation<TShape>(TShape shape, Point location) => (TShape)(object)(((Shape)(object)shape).SetLocationWpf(location));

    public TShape SetLocation<TShape>(TShape shape, TwoPoints location) => (TShape)(object)(((Line)(object)shape).SetLocationWpf(location));

    private TShape Modify<TShape, TArg>(TShape shape, Action<TShape, TArg> modifyFunc, TArg arg)
    {
        modifyFunc(shape, arg);
        return shape;
    }
}
