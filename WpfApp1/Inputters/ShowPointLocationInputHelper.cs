using CoreLib;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfLib;

namespace ConveyorApp.Inputters;

public class ShowThreePointCircleOnMouseLocationInputHelper : ShowShapeInputHelper<ShowThreePointCircleOnMouseLocationInputHelper, Shape>
{
    protected override Shape CreateShape() => Context.MainWindow.ShapeProvider.CreateCircle(default, default);
    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    public Point Point1 { get; set; }
    public Point Point2 { get; set; }

    private void Context_MouseMovedInCanvas(object sender, MouseEventArgs e)
    {
        var point = Context.GetSnappedCanvasPoint(e);
        if (Maths.GetCircleInfo((Point1, Point2, point), out var circInfo))
        {
            TmpShape.Visibility = System.Windows.Visibility.Visible;
            TmpShape.SetCenterLocation(circInfo.Center);
            TmpShape.Height = 2 * circInfo.Radius + 1;
            TmpShape.Width = 2 * circInfo.Radius + 1;
        }
        else
        {
            TmpShape.Visibility = System.Windows.Visibility.Hidden;
        }
    }

    internal static ShowThreePointCircleOnMouseLocationInputHelper Create(CanvasInputContext context, Point point1, Point point2)
    {
        var result = Create(context);
        result.Point1 = point1;
        result.Point2 = point2;
        return result;
    }
}

public abstract class ShowPointInputHelper<TThis> : ShowShapeInputHelper<TThis, Ellipse>
    where TThis : ShowPointInputHelper<TThis>, new()
{
    protected override Ellipse CreateShape() => Context.MainWindow.ShapeProvider.CreateTempPoint(default);
}

public abstract class ShowShapeInputHelper<TThis, TShape> : Inputter<TThis, Unit, CanvasInputContext>
    where TThis : ShowShapeInputHelper<TThis, TShape>, new()
    where TShape : Shape
{
    protected override void CleanupVirtual()
    {
        base.CleanupVirtual();
        Context.Canvas.Children.Remove(_TmpShape);
    }

    private TShape _TmpShape;

    protected abstract TShape CreateShape();

    protected TShape TmpShape
    {
        get
        {
            if (_TmpShape is null)
            {
                _TmpShape = CreateShape();
                Context.Canvas.Children.Add(_TmpShape);
            }
            return _TmpShape;
        }
    }
}

public class FixedPointInputHelper : ShowPointInputHelper<FixedPointInputHelper>
{
    private Point _Location;
    public Point Location
    {
        get => _Location;
        set => Func.Setter(ref _Location, value, () => TmpShape.SetCenterLocation(Location));
    }

    public static FixedPointInputHelper Create(CanvasInputContext context, Point location)
    {
        var result = Create(context);
        result.Location = location;
        return result;
    }
}

public class ShowMouseLocationInputHelper : ShowPointInputHelper<ShowMouseLocationInputHelper>
{
    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, MouseEventArgs e)
    {
        var point = Context.GetSnappedCanvasPoint(e);
        TmpShape.SetCenterLocation(point);
    }
}