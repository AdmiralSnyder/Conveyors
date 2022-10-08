using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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

public abstract class ConveyorAppApplicationObject<TShape> : CanvasableObject<ConveyorCanvasInfo, Canvas, ConveyorAppApplication, TShape>
    where TShape : FrameworkElement
{
    protected override void AddToCanvasVirtual(TShape shape) => Canvas.Children.Add(shape);
    protected override void SetTag(TShape shape, object tag) => shape.Tag = tag;
}

public abstract class CanvasableObject<TCanvasInfo, TCanvas, TApplication, TShape> : ApplicationObject<TApplication>
    where TApplication : IApplication
    where TCanvasInfo : CanvasInfo<TCanvas>
    where TCanvas : class
{
    protected TShape Shape { get; private set; }

    protected TCanvas? Canvas { get; private set; }
    protected TCanvasInfo CanvasInfo { get; private set; }
    protected abstract void SetTag(TShape shape, object tag);

    protected abstract TShape GetShape();
    protected abstract void AddToCanvasVirtual(TShape shape);
    
    public void AddToCanvas(TCanvasInfo canvasInfo)
    {
        Canvas = canvasInfo.Canvas;
        CanvasInfo = canvasInfo;
        Shape = GetShape();
        SetTag(Shape, this);
        AddToCanvasVirtual(Shape);
    }

}

public abstract class ApplicationObject<TApplication> : ISelectObject, IRefreshable, IAppObject<TApplication>
    where TApplication : IApplication
{
    public virtual string Text => Name;
    public string Name { get; set; }
    public virtual Vector[] SelectionBoundsPoints => null;
    public virtual ISelectObject? SelectionParent => null;
}

public class Line : ConveyorAppApplicationObject<Shape>
{
    public override string Text => "SomeLine123";

    public override Vector[] SelectionBoundsPoints => new[] { ReferencePoint1, ReferencePoint2 };

    public Point ReferencePoint1 { get; set; }
    public Point ReferencePoint2 { get; set; }

    public TwoPoints RefPoints => (ReferencePoint1, ReferencePoint2);
    public Vector Vector { get; set; }

    public bool IsVertical { get; set; }
    public double Slope { get; set; }
    public double OffsetY { get; set; }
    public Canvas? Canvas { get; private set; }

    protected override Shape GetShape() => CanvasInfo.ShapeProvider.CreateLine((ReferencePoint1, ReferencePoint2));

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
