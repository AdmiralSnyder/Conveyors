using CoreLib.Definition;
using UILib.Shapes;

namespace ConveyorLib.Objects;

public class LineSegment : ConveyorAppApplicationObject<LineSegment, IShape, LineDefinition, TwoPoints>, ILineDefinition
{
    public override string Text => "SomeLineSegment123";

    public override Vector[] GetSelectionBoundsPoints() => new[] { Definition.ReferencePoint1, Definition.ReferencePoint2 };

    protected override IShape GetShape() => CanvasInfo.ShapeProvider.CreateLineSegment(Definition.RefPoints);
}
