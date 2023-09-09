using CoreLib.Definition;
using UILib.Shapes;

namespace ConveyorLib.Objects;

public interface ICircleDefinition
{
    public CircleDefinition Definition { get; }
}

public class Circle: ConveyorAppApplicationObject<Circle, IShape, CircleDefinition, CircleDefinitionSource>, ICircleDefinition
{
    protected override IShape GetShape() => CanvasInfo.ShapeProvider.CreateCircle(Definition.CenterRadius.Center, Definition.CenterRadius.Radius);
    public override Vector[] GetSelectionBoundsPoints() => new[] 
    {
        Definition.CenterRadius.Center - (Definition.CenterRadius.Radius - 1, Definition.CenterRadius.Radius - 1), 
        Definition.CenterRadius.Center + (Definition.CenterRadius.Radius - 1, Definition.CenterRadius.Radius - 1)
    };
}
