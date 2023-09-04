using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using CoreLib;
using UILib;
using UILib.Shapes;
using WpfLib;

namespace ConveyorApp;

public class CanvasInputContext : InputContextBase
{
    private CanvasInfo _Canvas;
    public CanvasInfo Canvas 
    { 
        get => _Canvas;
        set => Func.Setter(ref _Canvas, value, RegisterCanvas); 
    }

    private void RegisterCanvas(CanvasInfo oldCanvasInfo, CanvasInfo newCanvasInfo)
    {
        if (oldCanvasInfo is { Canvas :  { } oldCanvas})
        {
            oldCanvas.MouseDown -= HandleMouseDown;
            oldCanvas.MouseUp -= HandleMouseUp;
            oldCanvas.MouseMove -= HandleMouseMove;
        }
        if (newCanvasInfo is { Canvas: { } newCanvas})
        { 
            newCanvas.MouseDown += HandleMouseDown;
            newCanvas.MouseUp += HandleMouseUp;
            newCanvas.MouseMove += HandleMouseMove;
        }
    }

    public TextBlock NotesLabel { get; set; }

    public override void SetCursor(Cursor cursor) => Canvas.Canvas.Cursor = cursor;

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

    protected override bool HandleMouseDownPanning(MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle && PanPoint is null)
        {
            PanPoint = GetWindowPoint(e);
            PanValue = ViewModel.PanValue;
            return true;
        }
        return false;
    }

    protected override bool HandleMouseMovePanning(MouseEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed && PanPoint.HasValue)
        {
            var diff = GetWindowPoint(e) - PanPoint.Value;

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

    public void SetLineEnd(ILine line, Point point)
    {
        line.X2 = point.X;
        line.Y2 = point.Y;
    }

    public ILine AddLine(Point from, Point to)
    {
        var line = ViewModel.ShapeProvider.CreateConveyorPositioningLine(((Point)from, (Point)to));
        Canvas.AddToCanvas(line);
        return line;
    }

    public IShape AddPoint(Point point)
    {
        var pointShape = ViewModel.ShapeProvider.CreatePoint(point);
        Canvas.AddToCanvas(pointShape);
        return pointShape;
    }

    protected override bool HandleMouseUpPanning(MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            PanPoint = null;
            return true;
        }
        return false;
    }

    protected override void HandleMouseDownVirtual(MouseButtonEventArgs e)
    {
        base.HandleMouseDownVirtual(e);
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var point = GetCanvasPoint(e);
            DoLeftMouseButtonClicked(new(point));
        }

        else if (e.RightButton == MouseButtonState.Pressed)
        {
            DoAbort();
        }
    }

    internal void RemoveShape(Shape centerPointShape) => Canvas.RemoveFromCanvas(centerPointShape);
}
