using System.Windows.Controls;
using System.Windows.Shapes;
using CoreLib.Definition;

namespace ConveyorLib.Objects;

public class LineSegment : ConveyorAppApplicationObject<LineSegment, Shape, LineDefinition, TwoPoints>, ILineDefinition
{
    public override string Text => "SomeLineSegment123";

    public override Vector[] GetSelectionBoundsPoints() => new[] { Definition.ReferencePoint1, Definition.ReferencePoint2 };

    protected override Shape GetShape() => CanvasInfo.ShapeProvider.CreateLineSegment(Definition.RefPoints);
}
