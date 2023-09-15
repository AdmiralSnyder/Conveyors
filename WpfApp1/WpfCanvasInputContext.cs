using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using CoreLib;
using UILib.Shapes;
using WpfLib;
using InputLib;

namespace ConveyorApp;

public class WpfCanvasInputContext : InputContextBase
{
    private WpfCanvasInfo _Canvas;
    public WpfCanvasInfo Canvas
    {
        get => _Canvas;
        set => Func.Setter(ref _Canvas, value, RegisterCanvas);
    }

    public override bool ShapesHaveEvents => true;

    public override void AddTempShape(IShape shape) => Canvas.AddToCanvas(shape);
    public override void RemoveTempShape(IShape shape) => Canvas.RemoveFromCanvas(shape);

    private void RegisterCanvas(WpfCanvasInfo oldCanvasInfo, WpfCanvasInfo newCanvasInfo)
    {
        if (oldCanvasInfo is { Canvas: { } oldCanvas })
        {
            oldCanvas.MouseDown -= MouseDownEventHandler;
            oldCanvas.MouseUp -= MouseUpEventHandler;
            oldCanvas.MouseMove -= MouseMoveEventHandler;
        }
        if (newCanvasInfo is { Canvas: { } newCanvas })
        {
            newCanvas.MouseDown += MouseDownEventHandler;
            newCanvas.MouseUp += MouseUpEventHandler;
            newCanvas.MouseMove += MouseMoveEventHandler;
        }
    }

    protected void MouseMoveEventHandler(object sender, MouseEventArgs e) => HandleMouseMove(sender, e);
    protected void MouseUpEventHandler(object sender, MouseButtonEventArgs e) => HandleMouseUp(sender, e);
    protected void MouseDownEventHandler(object sender, MouseButtonEventArgs e) => HandleMouseDown(sender, e);

    public override void SetCursor(InputCursors cursor) => Canvas.Canvas.Cursor = cursor switch
    {
        InputCursors.Normal => Cursors.Arrow,
        InputCursors.Hand => Cursors.Hand,
        InputCursors.Cross => Cursors.Cross,
        _ => throw new NotImplementedException("Missing cursor")
    };

    public override void CaptureMouse()
    {
        if (Canvas is { })
        {
            Mouse.Capture(Canvas.Canvas);
        }
    }

    public override void ReleaseMouseCapture() => Mouse.Capture(null);

    protected override void UserNotesChanged()
    {
        base.UserNotesChanged();
        if (ViewModel is { })
        {
            ViewModel.StatusBarHelpText = UserNotes;
        }
    }

    public void StartObjectPickingListener() => ViewModel.InputPickManager.ChosenObjectChanged += InputPickManager_ChosenObjectChanged;

    public void StopObjectPickingListener() => ViewModel.InputPickManager.ChosenObjectChanged -= InputPickManager_ChosenObjectChanged;

    private void InputPickManager_ChosenObjectChanged(object? sender, CoreLib.EventArgs<(ISelectable? SelObj, Point Point)> e)
    {
        if (e.Data.SelObj is ISelectObject so)
        {
            DoObjectPicked(so, e.Data.Point);
        }
    }

    protected override bool HandleMouseDownPanning(EventArgs e)
    {
        if (e is MouseButtonEventArgs mbea && mbea.ChangedButton == MouseButton.Middle && PanPoint is null)
        {
            PanPoint = GetWindowPoint(mbea);
            PanValue = ViewModel.PanValue;
            return true;
        }
        return false;
    }

    protected override bool HandleMouseMovePanning(EventArgs e)
    {
        if (e is MouseEventArgs mea && mea.MiddleButton == MouseButtonState.Pressed && PanPoint.HasValue)
        {
            var diff = GetWindowPoint(mea) - PanPoint.Value;

            ViewModel.PanValue = (PanValue.X + diff.X, PanValue.Y + diff.Y);
            return true;
        }
        return false;
    }

    private Point? PanPoint;
    private Point PanValue = new();
    private Rect snapGridWidthRectGodIHateWPF;

    private Point GetWindowPoint(MouseEventArgs e) => ViewModel.GetAbsolutePositionFunc(e);
    public Point GetCanvasPoint(MouseEventArgs e) => e.GetPosition(Canvas.Canvas).AsPoint();

    public Point GetSnappedCanvasPoint(MouseEventArgs e) => SnapPoint(GetCanvasPoint(e));






    //private const double SnapGridWidthDefault = 10;
    //public static readonly DependencyProperty SnapGridWidthProperty =
    //    DependencyProperty.Register(nameof(SnapGridWidth), typeof(double), typeof(MainWindow), new UIPropertyMetadata(SnapGridWidthDefault));

    public bool SnapToGrid { get; set; } = true;
    public MainWindowViewModel ViewModel { get; internal set; }

    public Point SnapPoint(Point point) => SnapPoint(point, SnapToGrid && !Keyboard.IsKeyDown(Key.LeftAlt));
    public Point SnapPoint(Point point, bool snap) => snap ? SnapPoint(point, snap, (int)ViewModel.SnapGridWidth) : point;
    public Point SnapPoint(Point point, bool snap, int snapGridWidth) => snap ? ((int)((point.X + snapGridWidth / 2) / snapGridWidth) * snapGridWidth, (int)((point.Y + snapGridWidth / 2) / snapGridWidth) * snapGridWidth) : point;

    public IShape AddPoint(Point point)
    {
        var pointShape = ViewModel.ShapeProvider.CreatePoint(point);
        Canvas.AddToCanvas(pointShape);
        return pointShape;
    }

    protected override bool HandleMouseUpPanning(EventArgs e)
    {
        if (e is MouseButtonEventArgs mbea && mbea.ChangedButton == MouseButton.Middle)
        {
            PanPoint = null;
            return true;
        }
        return false;
    }

    protected override void HandleMouseDownVirtual(EventArgs e)
    {
        base.HandleMouseDownVirtual(e);
        if (e is MouseButtonEventArgs mbea)
        {
            if (mbea.LeftButton == MouseButtonState.Pressed)
            {
                var point = GetCanvasPoint(mbea);
                DoLeftMouseButtonClicked(new(point));
            }
            else if (mbea.RightButton == MouseButtonState.Pressed)
            {
                DoAbort();
            }
        }
    }

    internal void RemoveShape(Shape centerPointShape) => Canvas.RemoveFromCanvas(centerPointShape);

    public override Point GetPoint(EventArgs args) => GetSnappedCanvasPoint((MouseEventArgs)args);
    public override Point GetPoint(Point point) => SnapPoint(point);

    public override bool IsShiftPressed() => Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

    public override bool IsRightClick(EventArgs args)
        => args is MouseButtonEventArgs mbea
        ? mbea.ChangedButton == MouseButton.Right
        : throw new NotImplementedException("unknown event args");

    public override bool IsMiddleClick(EventArgs args)
        => args is MouseButtonEventArgs mbea
        ? mbea.ChangedButton == MouseButton.Middle
        : throw new NotImplementedException("unknown event args");
}
