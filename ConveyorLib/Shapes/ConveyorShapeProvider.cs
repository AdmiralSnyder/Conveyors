using ConveyorLib.Shapes.Conveyor;
using ConveyorLib.Shapes.Standard;
using PointDef;
using PointDef.twopoints;
using System.Drawing;
using UILib.Behaviors;
using UILib.Shapes;

namespace ConveyorLib.Shapes;

public abstract class ConveyorShapeProvider<TEllipse, TLine, TPath> : IConveyorShapeProvider
    where TEllipse : IEllipse, new()
    where TLine : ILine, new()
    where TPath : IPath, new()
{
    public abstract void AddAdornedShapeLayer(IShape shape);

    public IEllipse CreateCircle(Point center, double radius)
        => StandardShapeSpecifications.CreateCircle<TEllipse>(center, radius)
        .WithSelectBehavior();

    public IEllipse CreateConveyorItemEllipse()
        => ConveyorShapeSpecifications.CreateConveyorItemEllipse<TEllipse>();

    public IEllipse CreateConveyorPointEllipse(Vector point, bool isFirst, bool isLast, bool isClockwise, bool isStraight, double size = 4)
        => ConveyorShapeSpecifications.CreateConveyorPointEllipse<TEllipse>(point, isFirst, isLast, isClockwise, isStraight, size);

    public ILine CreateLine(TwoPoints points)
        => StandardShapeSpecifications.CreateLine<TLine>(points)
        .WithSelectBehavior();

    public ILine CreateLineSegment(TwoPoints points)
        => StandardShapeSpecifications.CreateLineSegment<TLine>(points)
        .WithSelectBehavior();

    public IEllipse CreatePoint(Point point)
        => StandardShapeSpecifications.CreatePoint<TEllipse>(point)
        .WithSelectBehavior();

    public ILine CreateTempLine(TwoPoints points)
        => CreateLine(points).MarkAsTemporary();

    public IEllipse CreateTempPoint(Point point)
        => CreatePoint(point).MarkAsTemporary();

    public ILine CreateDebugLineSegment(TwoPoints points)
        => CreateLine(points).MarkAsDebug();
    public ILine CreateDebugThinLineSegment(TwoPoints points)
        => CreateLine(points).WithThinStroke().MarkAsDebug();

    public virtual IEllipse CreatePointMoveCircle(Point location, Action<IShape> moveCircleClicked) => CreateCircle(location, 7.5d).Modify(x =>
    {
        x.StrokeColor = Color.BurlyWood;
        x.StrokeThickness = 3;
    }).WithMouseBehavior(moveCircleClicked);

    public ILine CreateConveyorPositioningLine(TwoPoints value) =>
        CreateLineSegment(value).Modify(x =>
        {
            x.StrokeColor = Color.Black;
            x.StrokeThickness = 2;
        });

    public ILine CreateConveyorSegmentLaneLine(TwoPoints points)
        => CreateLineSegment(points).Modify(x =>
        {
            x.StrokeColor = Color.White;
            x.StrokeThickness = 1;
        });

    public ILine CreateConveyorSegmentLine(TwoPoints points)
        => CreateLineSegment(points).Modify(x =>
        {
            x.StrokeColor = Color.Red;
            x.StrokeThickness = 2;
        });

    public IPath CreateConveyorPointPath(IPathGeometry geometry, bool isLeft) => new TPath
    {
        StrokeColor = isLeft ? Color.Plum : Color.Tomato,
        Geometry = geometry,
    }
    .WithSelectBehavior();

    public IPath CreateCircleSectorArc(IPathGeometry geometry, bool isLeft) => new TPath
    {
        Geometry = geometry,
        StrokeColor = isLeft ? Color.Orange : Color.Teal,
        StrokeThickness = 2,
    }.WithSelectBehavior();

    public IPath CreatePath(IPathGeometry geometry) => new TPath
    {
        Geometry = geometry,
        StrokeColor = Color.Bisque
    }.WithSelectBehavior();

    public IShape CreateFillet(TwoPoints points, double radius)
    {
        var pg = GeometryProvider.CreatePathGeometry();
        pg.AddArcFigure(points.P1, points.P2, radius, 0, false, ArcSweepDirections.Clockwise);
        return CreateCircleSectorArc(pg, true);
    }

    public IPath CreateFreeHandLine(IEnumerable<Point> points)
    {
        var pg = GeometryProvider.CreatePathGeometry();
        if (points.Any())
        {
            var from = points.First();
            foreach(var point in points.Skip(1))
            {
                pg.AddLineFigure(from, point);
                from = point;
            }
        }
        return CreatePath(pg);
    }
}
