using ConveyorLib;
using PointDef;
using PointDef.twopoints;
using UILib.Shapes;

namespace ConveyorLibWeb;

public class ConveyorShapeProvider<TEllipse, TLine> : IConveyorShapeProvider
    where TEllipse : IEllipse, new()
    where TLine : ILine, new()
{
    public void AddAdornedShapeLayer(IShape shape) => throw new NotImplementedException();

    public IShape CreateCircle(Point center, double radius) => ConveyorShapeSpecifications.CreateCircle<TEllipse>(center, radius);

    public IEllipse CreateConveyorItemEllipse() => ConveyorShapeSpecifications.CreateConveyorItemEllipse<TEllipse>();

    public IEllipse CreateConveyorPointEllipse(V2d point, bool isFirst, bool isLast, bool isClockwise, bool isStraight, double size = 4)
        => ConveyorShapeSpecifications.CreateConveyorPointEllipse<TEllipse>(point, isFirst, isLast, isClockwise, isStraight, size);

    public IPath CreateConveyorPointPath(IPathGeometry geometry, bool isLeft) => throw new NotImplementedException();

    public ILine CreateConveyorPositioningLine(TwoPoints<V2d> value) => throw new NotImplementedException();

    public ILine CreateConveyorSegmentLaneLine(TwoPoints<V2d> points) => throw new NotImplementedException();

    public ILine CreateConveyorSegmentLine(TwoPoints<V2d> points) => throw new NotImplementedException();

    public ILine CreateDebugLineSegment(TwoPoints<V2d> points) => throw new NotImplementedException();

    public ILine CreateDebugThinLineSegment(TwoPoints<V2d> points) => throw new NotImplementedException();

    public IShape CreateFillet(TwoPoints<V2d> points, double radius) => throw new NotImplementedException();

    public ILine CreateLine(TwoPoints points) => ConveyorShapeSpecifications.CreateLine<TLine>(points);

    public ILine CreateLineSegment(TwoPoints<V2d> points) => throw new NotImplementedException();

    public IShape CreatePoint(Point point) => ConveyorShapeSpecifications.CreatePoint<TEllipse>(point);

    public IEllipse CreatePointMoveCircle(V2d location, Action<IShape> moveCircleClicked) => throw new NotImplementedException();

    public ILine CreateTempLine(TwoPoints<V2d> points) => throw new NotImplementedException();

    public IEllipse CreateTempPoint(V2d point) => throw new NotImplementedException();

    public void RegisterSelectBehavior(Action<IShape> selectShapeAction) => throw new NotImplementedException();
}
