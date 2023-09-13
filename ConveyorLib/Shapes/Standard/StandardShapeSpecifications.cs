using PointDef.twopoints;
using UILib.Shapes;
using static ConveyorLib.Shapes.ShapeSpecificationDefaults;

namespace ConveyorLib.Shapes.Standard;

public static class StandardShapeSpecifications
{
    public static TShape CreatePoint<TShape>(Point point)
        where TShape : IEllipse, new()
        => new TShape()
        {
            Width = DefaultPointDiameter,
            Height = DefaultPointDiameter,
            FillColor = DefaultPointColor,
        }.SetCenterLocation(point);

    public static TShape CreateCircle<TShape>(Point center, double radius)
        where TShape : IEllipse, new()
        => new TShape()
        {
            Width = radius * 2,
            Height = radius * 2,
            StrokeColor = DefaultCircleColor,
        }.SetCenterLocation(center);

    internal static TShape CreateLineSegment<TShape>(TwoPoints points)
        where TShape : ILine, new() => new()
        {
            X1 = points.P1.X,
            Y1 = points.P1.Y,
            X2 = points.P2.X,
            Y2 = points.P2.Y,
            StrokeThickness = DefaultLineStrokeThickness,
            StrokeColor = DefaultLineColor,
        };

    internal static TShape CreateLine<TShape>(TwoPoints points)
        where TShape : ILine, new()
    {
        var vector = points.P2 - points.P1;
        var start = points.P1;
        var end = points.P2;
        //       -----_-----
        var len = points.Length();
        if (len > 0 && len < 1000)
        {
            var factor = 1000 / len;
            start = points.P1 - vector.Multiply(factor);
            end = points.P2 + vector.Multiply(factor);

        }
        return new()
        {
            X1 = start.X,
            Y1 = start.Y,
            X2 = end.X,
            Y2 = end.Y,
            StrokeThickness = DefaultLineStrokeThickness,
            StrokeColor = DefaultLineColor,
        };
    }
}
