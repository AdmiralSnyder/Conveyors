using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ConveyorLib.Objects;

public class Line : ConveyorAppApplicationObject<Shape>
{

    public LineDefinition Definition { get; private set; }

    public override string Text => "SomeLine123";

    public override Vector[] SelectionBoundsPoints => new[] { Definition.ReferencePoint1, Definition.ReferencePoint2 };

    public Canvas? Canvas { get; private set; }

    protected override Shape GetShape() => CanvasInfo.ShapeProvider.CreateLine(Definition.RefPoints);

    public static Line Create(TwoPoints points)
    {
        return new Line
        {
            Definition = new((points.P1, points.P2))
        };
    }
}
