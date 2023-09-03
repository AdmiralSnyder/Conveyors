using UILib.Shapes;

namespace ConveyorLib.Objects;

/// <summary>
/// a thing that smooths out the connection corner of lines.
/// </summary>
/// <image url="../../docs/fillet.gif"></image>
public class Fillet : ConveyorAppApplicationObject<Fillet, IShape, (TwoPoints Points, double Radius)>
{
    public override string Text => "SomeFillet123";
    public override Vector[] GetSelectionBoundsPoints() => new[] { Source.Points.P1, Source.Points.P1 };

    //public Point ReferencePoint1 { get; set; }
    //public Point ReferencePoint2 { get; set; }
    //public double Radius { get; set; }

    protected override IShape GetShape() => CanvasInfo.ShapeProvider.CreateFillet(Source.Points, Source.Radius);

    //public static Fillet Create((TwoPoints points, double radius) tuple) => new()
    //{
    //    ReferencePoint1 = tuple.points.P1,
    //    ReferencePoint2 = tuple.points.P2,
    //    Radius = tuple.radius,
    //};
}
