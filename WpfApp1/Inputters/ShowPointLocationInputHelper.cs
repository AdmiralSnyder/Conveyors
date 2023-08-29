using CoreLib;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UILib;
using WpfLib;

namespace ConveyorApp.Inputters;

public abstract class ShowDynamicCircleInputHelper<TThis> : ShowDynamicShapeInputHelper<TThis>
    where TThis : ShowDynamicCircleInputHelper<TThis>, new()
{
    protected override Shape CreateShape() => Context.MainWindow.ViewModel.ShapeProvider.CreateCircle(default, default);
    protected void UpdateCircle(Point center, double radius)
    {
        TmpShape.SetCenterLocation(center);
        TmpShape.Height = 2 * radius + 1;
        TmpShape.Width = 2 * radius + 1;

    }
}
public abstract class ShowDynamicShapeInputHelper<TThis> : ShowShapeInputHelper<TThis, Shape>
    where TThis : ShowDynamicShapeInputHelper<TThis>, new()
{
    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, MouseEventArgs e)
    {
        var point = Context.GetSnappedCanvasPoint(e);
        TmpShape.Visibility = UpdateMousePoint(point)
            ? System.Windows.Visibility.Visible
            : System.Windows.Visibility.Hidden;
    }
    protected abstract bool UpdateMousePoint(Point point);
}

public class ShowCircleByRadiusInputHelper : ShowDynamicCircleInputHelper<ShowCircleByRadiusInputHelper>
{
    public Point Center { get; set; }

    internal static ShowCircleByRadiusInputHelper Create(CanvasInputContext context, Point point1)
    {
        var result = Create(context);
        result.Center = point1;
        return result;
    }

    protected override bool UpdateMousePoint(Vector point)
    {
        if (point != Center)
        {
            UpdateCircle(Center, (Center - point).Length());
            return true;
        }
        else return false;
    }
}


public class ShowCircleByDiameterInputHelper : ShowDynamicCircleInputHelper<ShowCircleByDiameterInputHelper>
{

    public Point Point1 { get; set; }

    internal static ShowCircleByDiameterInputHelper Create(CanvasInputContext context, Point point1)
    {
        var result = Create(context);
        result.Point1 = point1;
        return result;
    }

    protected override bool UpdateMousePoint(Vector point)
    {
        if (Maths.GetCircleInfoByDiameter((Point1, point), out var info, out _))
        {
            UpdateCircle(info.Center, info.Radius);
            return true;
        }
        else return false;
    }
}

public class ShowThreePointCircleOnMouseLocationInputHelper : ShowDynamicCircleInputHelper<ShowThreePointCircleOnMouseLocationInputHelper>
{
    protected override Shape CreateShape() => Context.MainWindow.ViewModel.ShapeProvider.CreateCircle(default, default);

    public Point Point1 { get; set; }
    public Point Point2 { get; set; }

    protected override bool UpdateMousePoint(Vector point)
    {
        if (Maths.GetCircleInfoByThreePoints((Point1, Point2, point), out var circInfo))
        {
            UpdateCircle(circInfo.Center, circInfo.Radius);
            return true;
        }
        else return false;
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
    protected override Ellipse CreateShape() => Context.MainWindow.ViewModel.ShapeProvider.CreateTempPoint(default);
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

    private TShape? _TmpShape;

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

public abstract class ShowLineFromToInputHelper<TThis> : ShowShapeInputHelper<TThis, Line>
where TThis : ShowLineFromToInputHelper<TThis>, new()
{
    private Point _StartPoint;

    protected Point StartPoint
    {
        get => _StartPoint;
        set => Func.Setter(ref _StartPoint, value, () =>
        {
            TmpShape.X1 = StartPoint.X;
            TmpShape.Y1 = StartPoint.Y;
        });
    }

    private Point _EndPoint;

    protected Point EndPoint
    {
        get => _EndPoint;
        set => Func.Setter(ref _EndPoint, value, () =>
        {
            TmpShape.X2 = EndPoint.X;
            TmpShape.Y2 = EndPoint.Y;
        });
    }

    protected override Line CreateShape() => Context.MainWindow.ViewModel.ShapeProvider.CreateTempLine(default);
}

public class ShowLineFromToFixedInputHelper : ShowLineFromToInputHelper<ShowLineFromToFixedInputHelper>
{
    public static ShowLineFromToFixedInputHelper Create(CanvasInputContext context, Point point1, Point point2)
    {
        var result = Create(context);
        result.StartPoint = point1;
        result.EndPoint = point2;
        return result;
    }
}

public class ShowLineFromToMouseInputHelper : ShowLineFromToInputHelper<ShowLineFromToMouseInputHelper>
{
    public static ShowLineFromToMouseInputHelper Create(CanvasInputContext context, Point point1)
    {
        var result = Create(context);
        result.StartPoint = point1;
        result.EndPoint = point1;
        return result;
    }

    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, MouseEventArgs e)
    {
        var point = Context.GetSnappedCanvasPoint(e);
        EndPoint = point;
    }
}

public class ShowUserNotesInputHelper : Inputter<ShowUserNotesInputHelper, InputContextBase>
{
    public static ShowUserNotesInputHelper Create(InputContextBase context, string userNotes)
    {
        context.UserNotes = userNotes;
        return Create(context);
    }

    protected override void CleanupVirtual()
    {
        base.CleanupVirtual();
        Context.UserNotes = "";
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

public class ShowPickedSelectableInputHelper : Inputter<ShowPickedSelectableInputHelper, CanvasInputContext>
{
    private CanvasObjectHighlighter? CanvasObjectHighlighter { get; set; }
    protected override void CleanupVirtual()
    {
        base.CleanupVirtual();
        CanvasObjectHighlighter!.SelectObject = null;
        CanvasObjectHighlighter = null;
    }

    //private ISelectObject SelectObject { get; set; }

    //public ISelectObject SelectObject 
    //{
    //    get => _SelectObject; 
    //    set => Func.Setters(ref _SelectObject, value); 
    //}

    public static ShowPickedSelectableInputHelper Create(CanvasInputContext context, ISelectObject selectObject)
    {
        var result = Create(context);
        result.CanvasObjectHighlighter = CanvasObjectHighlighter.Create(context.Canvas, selectObject);
        return result;
    }
}

public class ShowCalculatedPointInputHelper : ShowPointInputHelper<ShowCalculatedPointInputHelper>
{
    private Func<Vector, Vector> CalculationOnMouse;

    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    public static ShowCalculatedPointInputHelper Create(CanvasInputContext context, Func<Point, Point> calculationOnMouse)
    {
        var result = Create(context);
        result.CalculationOnMouse = calculationOnMouse;
        return result;
    }

    private void Context_MouseMovedInCanvas(object sender, MouseEventArgs e)
    {
        var mousePoint = Context.GetSnappedCanvasPoint(e);
        var point = CalculationOnMouse(mousePoint);
        TmpShape.SetCenterLocation(point);
    }
}