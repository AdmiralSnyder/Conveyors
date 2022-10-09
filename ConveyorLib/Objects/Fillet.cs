using System.Windows.Shapes;

namespace ConveyorLib.Objects;

/// <summary>
/// a thing that smooths out the connection corner of lines.
/// </summary>
/// <image url="../../docs/fillet.gif"></image>
public class Fillet : ConveyorAppApplicationObject<Shape>
{
    public override string Text => "SomeFillet123";
    public override Vector[] SelectionBoundsPoints => new[] { ReferencePoint1, ReferencePoint2};

    public Point ReferencePoint1 { get; set; }
    public Point ReferencePoint2 { get; set; }
    public double Radius { get; set; }

    protected override Shape GetShape() => CanvasInfo.ShapeProvider.CreateFillet((ReferencePoint1, ReferencePoint2), Radius);

    public static Fillet Create(TwoPoints points, double radius) => new()
    {
        ReferencePoint1 = points.P1,
        ReferencePoint2 = points.P2,
        Radius = radius,
    };
}
