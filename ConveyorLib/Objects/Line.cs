using PointDef.twopoints;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ConveyorLib.Objects;

public class Line : ISelectObject, IRefreshable, IAppObject<ConveyorAppApplication>
{
    public string Text => "SomeLine123";

    public Vector[] SelectionBoundsPoints => new[] { ReferencePoint1, ReferencePoint2 };

    public ISelectObject? SelectionParent => null;

    public Point ReferencePoint1 { get; set; }
    public Point ReferencePoint2 { get; set; }
    public Vector Vector { get; set; }

    public bool IsVertical { get; set; }
    public double Slope { get; set; }
    public double OffsetY { get; set; }
    public Canvas? Canvas { get; private set; }
    public Shape? Shape { get; set; }

    public void AddToCanvas(ConveyorCanvasInfo canvasInfo)
    {
        Canvas = canvasInfo.Canvas;
        Shape = canvasInfo.ShapeProvider.CreateLine((ReferencePoint1, ReferencePoint2));
        Shape.Tag = this;

        canvasInfo.Canvas.Children.Add(Shape);
    }

    public static Line Create(TwoPoints points)
    {
        bool isVertical = points.P1.X == points.P2.X;
        var vector = points.P2 - points.P1;
        double? slope = null;
        return new Line
        {
            ReferencePoint1 = points.P1,
            ReferencePoint2 = points.P2,
            Vector = vector,
            IsVertical = isVertical,
            Slope = isVertical ? double.NaN : (slope = Maths.GetSlope(vector)).Value,
            OffsetY = isVertical ? double.NaN : Maths.GetOffsetY(points.P2, slope!.Value),
        };
    }
}
