using CoreLib.Definition;
using PointDef.twopoints;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ConveyorLib.Objects;

public interface ILineDefinition
{
    public LineDefinition Definition { get; }
}

public class Line : ConveyorAppApplicationObject<Line, Shape, LineDefinition, TwoPoints>, ILineDefinition
{
    public override string Text => "SomeLine123";

    public override Vector[] GetSelectionBoundsPoints() => new[] { Definition.ReferencePoint1, Definition.ReferencePoint2 };

    protected override Shape GetShape() => CanvasInfo.ShapeProvider.CreateLine(Definition.RefPoints);
}

