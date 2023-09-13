using System.Drawing;
using UILib.Shapes;

namespace ConveyorLib.Shapes.Conveyor;

public static class ConveyorShapeSpecifications
{
    private const double ConveyorItemSize = 3;

    public static TShape CreateConveyorItemEllipse<TShape>()
        where TShape : IEllipse, new()
        => new()
        {
            Width = ConveyorItemSize,
            Height = ConveyorItemSize,
            StrokeColor = Color.Blue
        };

    public static TShape CreateConveyorPointEllipse<TShape>(Point point, bool isFirst, bool isLast, bool isClockwise, bool isStraight, double size = 4)
        where TShape : IEllipse, new()
        => new TShape()
        {
            Width = size,
            Height = size,
            FillColor = isLast ? Color.Red
            : isFirst ? Color.Cyan
            : isClockwise ? Color.Purple
            : isStraight ? Color.Peru
            : Color.Blue,
        }.SetCenterLocation(point);
}
