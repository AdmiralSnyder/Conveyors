using UILib.Shapes;

namespace ConveyorLib;

public interface IConveyorShapeProvider : IShapeProvider
{
    IEllipse CreateConveyorItemEllipse();
    void AddAdornedShapeLayer(IShape shape);
    ILine CreateConveyorPositioningLine(TwoPoints value);
    IShape CreatePoint(Point point);
    IEllipse CreatePointMoveCircle(Point location, Action<IShape> moveCircleClicked);
    IShape CreateCircle(Point center, double radius);
    IEllipse CreateTempPoint(Point point);
    ILine CreateTempLine(TwoPoints points);
    void RegisterSelectBehavior(Action<IShape> selectShapeAction);
    IEllipse CreateConveyorPointEllipse(Point point, bool isFirst, bool isLast, bool isClockwise, bool isStraight, double size = 4d);
    IPath CreateConveyorPointPath(IPathGeometry geometry, bool isLeft);
    ILine CreateConveyorSegmentLine(TwoPoints points);
    ILine CreateConveyorSegmentLaneLine(TwoPoints points);
    IShape CreateFillet(TwoPoints points, double radius);
    ILine CreateLine(TwoPoints points);
    ILine CreateLineSegment(TwoPoints points);
    ILine CreateDebugThinLineSegment(TwoPoints points);
    ILine CreateDebugLineSegment(TwoPoints points);

    IPathGeometry CreatePathGeometry();
}
