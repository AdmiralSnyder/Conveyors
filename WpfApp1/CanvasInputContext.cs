using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using CoreLib;
using UILib;

namespace ConveyorApp;

public class CanvasInputContext : InputContextBase
{
    private Canvas _Canvas;
    public Canvas Canvas 
    { 
        get => _Canvas;
        set => Func.Setter(ref _Canvas, value, RegisterCanvas); 
    }

    private void RegisterCanvas(Canvas oldCanvas, Canvas newCanvas)
    {
        if (oldCanvas is not null)
        {
            oldCanvas.MouseDown -= HandleMouseDown;
            oldCanvas.MouseUp -= HandleMouseUp;
            oldCanvas.MouseMove -= HandleMouseMove;
        }
        if (newCanvas is not null)
        { 
            newCanvas.MouseDown += HandleMouseDown;
            newCanvas.MouseUp += HandleMouseUp;
            newCanvas.MouseMove += HandleMouseMove;
        }
    }

    public TextBlock NotesLabel { get; set; }

    public override void SetCursor(Cursor cursor)
    {
        Canvas.Cursor = cursor;
    }

    public override void CaptureMouse()
    {
        if (Canvas is { })
        {
            Mouse.Capture(Canvas);
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

    public void StartObjectPickingListener() => MainWindow.ViewModel.InputPickManager.ChosenObjectChanged += InputPickManager_ChosenObjectChanged;

    public void StopObjectPickingListener() => MainWindow.ViewModel.InputPickManager.ChosenObjectChanged -= InputPickManager_ChosenObjectChanged;

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
            PanValue = new(MainWindow.CanvasTranslateTransform.X, MainWindow.CanvasTranslateTransform.Y);
            return true;
        }
        return false;
    }

    protected override bool HandleMouseMovePanning(MouseEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed && PanPoint.HasValue)
        {
            var diff = GetWindowPoint(e) - PanPoint.Value;

            MainWindow.CanvasTranslateTransform.X = PanValue.X + diff.X;
            MainWindow.CanvasTranslateTransform.Y = PanValue.Y + diff.Y;
            return true;
        }
        return false;
    }

    private Point? PanPoint;
    private Point PanValue = new();
    private Point GetWindowPoint(MouseEventArgs e) => e.GetPosition(MainWindow);
    public Point GetCanvasPoint(MouseEventArgs e) => e.GetPosition(Canvas);

    public Point GetSnappedCanvasPoint(MouseEventArgs e) => SnapPoint(GetCanvasPoint(e));

    private const int SnapGridWidthDefault = 10;
    public static readonly DependencyProperty SnapGridWidthProperty =
        DependencyProperty.Register(nameof(SnapGridWidth), typeof(int), typeof(MainWindow), new UIPropertyMetadata(SnapGridWidthDefault));

    public int SnapGridWidth { get; set; } = SnapGridWidthDefault;
    public bool SnapToGrid { get; set; } = true;
    public MainWindowViewModel ViewModel { get; internal set; }

    public Point SnapPoint(Point point) => SnapPoint(point, SnapToGrid && !Keyboard.IsKeyDown(Key.LeftAlt));
    public Point SnapPoint(Point point, bool snap) => snap ? SnapPoint(point, snap, SnapGridWidth) : point;
    public Point SnapPoint(Point point, bool snap, int snapGridWidth) => snap ? ((int)((point.X + snapGridWidth / 2) / snapGridWidth) * snapGridWidth, (int)((point.Y + snapGridWidth / 2) / snapGridWidth) * snapGridWidth) : point;

    public void SetLineEnd(Line line, Point point)
    {
        line.X2 = point.X;
        line.Y2 = point.Y;
    }

    public Line AddLine(Point from, Point to)
    {
        var line = MainWindow.ViewModel.ShapeProvider.CreateConveyorPositioningLine(((Point)from, (Point)to));
        Canvas.Children.Add(line);
        return line;
    }

    public Shape AddPoint(Point point)
    {
        var pointShape = MainWindow.ViewModel.ShapeProvider.CreatePoint(point);
        Canvas.Children.Add(pointShape);
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

    internal void RemoveShape(Shape centerPointShape)
    {
        Canvas.Children.Remove(centerPointShape);
    }
}
