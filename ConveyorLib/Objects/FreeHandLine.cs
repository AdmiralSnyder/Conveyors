using UILib.Shapes;

namespace ConveyorLib.Objects;

public class FreeHandLine : ConveyorAppApplicationObject<FreeHandLine, IShape, IEnumerable<Point>>
{
    protected override IShape GetShape() => CanvasInfo.ShapeProvider.CreateFreeHandLine(Source);
}
