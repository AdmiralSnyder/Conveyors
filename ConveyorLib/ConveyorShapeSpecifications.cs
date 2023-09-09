using System.Drawing;
using PointDef.twopoints;
using UILib;
using UILib.Shapes;

namespace ConveyorLibWeb;

public static class ConveyorShapeSpecifications
{
    public static TShape CreatePoint<TShape>(Point point)
        where TShape : IEllipse, new()
    {
        var shape = new TShape()
        {
            Width = 3,
            Height = 3,
            FillColor = Color.Magenta,
        };
        shape.SetCenterLocation(point);
        return shape;
    }

    public static TShape CreateCircle<TShape>(Point center, double radius)
        where TShape : IEllipse, new()
    {
        TShape shape = new()
        {
            Width = radius * 2,
            Height = radius * 2,
            StrokeColor = Color.Magenta
        };
        shape.SetCenterLocation(center);
        return shape;
    }

    private const double ItemSize = 3;

    public static TShape CreateConveyorItemEllipse<TShape>()
        where TShape : IEllipse, new()
        => new()
        {
            Width = ItemSize,
            Height = ItemSize,
            StrokeColor = Color.Blue
        };

    public static TShape CreateConveyorPointEllipse<TShape>(Point point, bool isFirst, bool isLast, bool isClockwise, bool isStraight, double size = 4)
        where TShape : IEllipse, new()
    {
        TShape shape = new()
        {
            Width = size,
            Height = size,
            FillColor = isLast ? Color.Red
            : isFirst ? Color.Cyan
            : isClockwise ? Color.Purple
            : isStraight ? Color.Peru
            : Color.Blue,
        };
        shape.SetCenterLocation(point);
        return shape;
    }

    internal static TShape CreateLine<TShape>(TwoPoints<Vector> points)
        where TShape : ILine, new()
        => new() 
        {
            X1 = points.P1.X,
            Y1 = points.P1.Y,
            X2 = points.P2.X,
            Y2 = points.P2.Y,
        };
}
