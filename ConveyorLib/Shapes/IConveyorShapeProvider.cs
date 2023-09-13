using UILib.Shapes;

namespace ConveyorLib.Shapes;

public interface IConveyorShapeProvider : IShapeProvider
{
    IEllipse CreateConveyorItemEllipse();
    void AddAdornedShapeLayer(IShape shape);
    ILine CreateConveyorPositioningLine(TwoPoints value);
    IEllipse CreatePoint(Point point);
    IEllipse CreatePointMoveCircle(Point location, Action<IShape> moveCircleClicked);
    IEllipse CreateCircle(Point center, double radius);
    IEllipse CreateTempPoint(Point point);
    ILine CreateTempLine(TwoPoints points);
    IEllipse CreateConveyorPointEllipse(Point point, bool isFirst, bool isLast, bool isClockwise, bool isStraight, double size = 4d);
    IPath CreateConveyorPointPath(IPathGeometry geometry, bool isLeft);
    ILine CreateConveyorSegmentLine(TwoPoints points);
    ILine CreateConveyorSegmentLaneLine(TwoPoints points);
    IShape CreateFillet(TwoPoints points, double radius);
    ILine CreateLine(TwoPoints points);
    ILine CreateLineSegment(TwoPoints points);
    ILine CreateDebugThinLineSegment(TwoPoints points);
    ILine CreateDebugLineSegment(TwoPoints points);
}
