using CoreLib.Definition;
using UILib.Shapes;

namespace ConveyorLib.Objects;

public interface ILineDefinition
{
    public LineDefinition Definition { get; }
}

public class Line : ConveyorAppApplicationObject<Line, IShape, LineDefinition, TwoPoints>, ILineDefinition
{
    public override string Text => "SomeLine123";

    public override Vector[] GetSelectionBoundsPoints() => new[] { Definition.ReferencePoint1, Definition.ReferencePoint2 };

    protected override IShape GetShape() => CanvasInfo.ShapeProvider.CreateLine(Definition.RefPoints);
}

