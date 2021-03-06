using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfLib;

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        ShapeProvider = new() { SelectBehaviour = SelectShapeAction };
        this.DataContext = this;
        SelectionManager = new()
        {
            UpdateBoundingBox = ShowSelectionBoundingBox
        };
        InitializeComponent();
        AutoRoot = ConveyorAutomationObject.CreateAutomationObject(out var context);
        AutoRoot.Init((TheCanvas, ShapeProvider));

        context.LogAction = s => textEditor2.Dispatcher.Invoke(() => textEditor2.AppendText(s + Environment.NewLine));
        ScriptRunner.InitializeScriptingEnvironment(AutoRoot, Dispatcher, RunB);

    }



    public IGeneratedConveyorAutomationObject AutoRoot { get; }

    public static readonly DependencyProperty SnapGridWidthProperty =
        DependencyProperty.Register(nameof(SnapGridWidth), typeof(int), typeof(MainWindow), new UIPropertyMetadata(10));

    public int SnapGridWidth
    {
        get => (int)GetValue(SnapGridWidthProperty);
        set => SetValue(SnapGridWidthProperty, value);
    }

    //public static readonly DependencyProperty SelectedObjectProperty =
    //   DependencyProperty.Register(nameof(SelectedObject), typeof(ISelectObject), typeof(MainWindow), new UIPropertyMetadata(null, SelectedObjectPropertyChanged));

    //static void SelectedObjectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //    ((MainWindow)d).ShowSelectionBoundingBox((ISelectObject)e.NewValue);
    //}

    public SelectionManager SelectionManager { get; set; }

    private void SelectShapeAction(Shape shape)
    {
        var oldSelectedObject = SelectionManager.SelectedObject;
        if (SelectionManager.SelectMode && shape.Tag is ISelectObject selectObject)
        {
            if (SelectionManager.HierarchicalSelection)
            {
                SelectionManager.SelectedObject = selectObject.FindPredecessorInPath(oldSelectedObject);                
            }
            else
            {
                SelectionManager.SelectedObject = selectObject;
            }
        }
    }

    private Rectangle SelectionRect;

    private void ShowSelectionBoundingBox(ISelectObject? selectObject)
    {
        if (SelectionRect is not null)
        {
            TheCanvas.Children.Remove(SelectionRect);
        }
        if (selectObject is null) return;

        var boundingRect = Maths.GetBoundingRectTopLeftSize(selectObject.SelectionBoundsPoints);
        SelectionRect = new()
        {
            Width = boundingRect.P2.X + 8,
            Height = boundingRect.P2.Y + 8,
            Stroke = Brushes.Moccasin,
            StrokeDashArray = new(new[] { 1d, 2d }),
            SnapsToDevicePixels = true,
            RadiusX = 2,
            RadiusY = 2,
        };
        SelectionRect.SetLocation(boundingRect.P1.Subtract((4, 4)));
        TheCanvas.Children.Add(SelectionRect);
    }

    private void AddConveyorB_Click(object sender, RoutedEventArgs e) => InputState = InputState.SelectFirstPoint;

    private InputState _InputState;
    private InputState InputState
    {
        get => _InputState;
        set
        {
            _InputState = value;
            switch (_InputState)
            {
                case InputState.SelectFirstPoint:
                    TheCanvas.Cursor = Cursors.Cross;
                    Mouse.Capture(TheCanvas);
                    NotesLabel.Text = "Please select the starting point.";
                    break;
                case InputState.SelectLastPoint:
                    TheCanvas.Cursor = Cursors.Cross;
                    NotesLabel.Text = "Please select the ending point.";
                    break;
                case InputState.None:
                    TheCanvas.Cursor = Cursors.Arrow;
                    Mouse.Capture(null);
                    NotesLabel.Text = "Click around. Have fun!";
                    break;
            }
        }
    }

    private readonly Stack<Line> TempLines = new();
    private Point? PanPoint;
    private Point PanValue = new();
    
    private void TheCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle && PanPoint is null)
        {
            PanPoint = GetWindowPoint(e);
            PanValue = new(CanvasTranslateTransform.X, CanvasTranslateTransform.Y);
            return;
        }

        var modifier = Keyboard.Modifiers;
        var isShiftPressed = modifier.HasFlag(ModifierKeys.Shift);
        switch (InputState)
        {
            case InputState.None: return;
            case InputState.SelectFirstPoint:
                switch (AddPoint())
                {
                    case ActionResults.Continue:
                    case ActionResults.Finish:
                        InputState = InputState.SelectLastPoint;
                        break;
                    case ActionResults.Abort:
                    case ActionResults.AbortAll:
                        AbortAll();
                        break;
                }
                break;
            case InputState.SelectLastPoint:
                switch (AddPoint())
                {
                    case ActionResults.Continue:
                        break;
                    case ActionResults.Finish:
                        Finish();
                        break;
                    case ActionResults.Abort:
                        Abort(true);
                        break;
                    case ActionResults.AbortAll:
                        AbortAll();
                        break;
                }
                break;
            case InputState.MovePoint:
                {
                    if (MoveShapes.FirstOrDefault() is { Tag: ConveyorPoint point })
                    {
                        point.Location = SnapPoint(GetCanvasPoint(e));
                    }
                    InputState = InputState.None;

                    foreach (var shape in MoveShapes)
                    {
                        TheCanvas.Children.Remove(shape);
                    }
                    MoveShapes.Clear();
                    break;
                }
        }

        void Abort(bool abortAllIfEmpty = false)
        {
            if (TempLines.TryPop(out var last))
            {
                TheCanvas.Children.Remove(last);
            }
            if (TempLines.TryPeek(out last))
            {
                SetLineEnd(last, SnapPoint(GetCanvasPoint(e)));
            }
            else
            {
                if (abortAllIfEmpty)
                {
                    InputState = InputState.None;
                }
            }
        }

        void AbortAll()
        {
            foreach (var line in TempLines)
            {
                TheCanvas.Children.Remove(line);
            }
            TempLines.Clear();
            InputState = InputState.None;
        }

        ActionResults AddPoint()
        {
            if (e.ChangedButton == MouseButton.Right) return isShiftPressed
                ? ActionResults.AbortAll
                : ActionResults.Abort;

            var point = SnapPoint(GetCanvasPoint(e));
            TempLines.Push(AddLine(point, point)); // das ist geschummelt, damit ich nicht umständlich Zustände speichern muss

            return isShiftPressed ? ActionResults.Continue : ActionResults.Finish;
        }

        void Finish()
        {
            Point lastPoint = (double.NaN, double.NaN);
            List<Point> points = new();
            foreach (var line in TempLines.Reverse())
            {
                TheCanvas.Children.Remove(line);
                line.Stroke = Brushes.Red;
                if (line.X1 != line.X2 || line.Y1 != line.Y2)
                {
                    AddPoint((line.X1, line.Y1));
                    AddPoint((line.X2, line.Y2));
                }
            }

            void AddPoint(Point p)
            {
                if (p != lastPoint)
                {
                    points.Add(p);
                }
                lastPoint = p;
            }

            // TODO reverse?


            TempLines.Clear();

            InputState = InputState.None;

            AutoRoot.AddConveyor(points, IsRunning, int.TryParse(LanesCountTB.Text, out var lanesCnt) ? Math.Max(lanesCnt, 1) : 1);
        }
    }

    //private void OnPropertyChanged(string propertyName)
    //{
    //    if (this.PropertyChanged != null)
    //    {
    //        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}


    private ConveyorShapeProvider ShapeProvider;
    public Line AddLine(Point from, Point to)
    {
        var line = ShapeProvider.CreateConveyorPositioningLine(((Point)from, (Point)to));
        TheCanvas.Children.Add(line);
        return line;
    }

    private void SetLineEnd(Line line, Point point)
    {
        line.X2 = point.X;
        line.Y2 = point.Y;
    }

    private Point GetWindowPoint(MouseEventArgs e) => e.GetPosition(this);
    private Point GetCanvasPoint(MouseEventArgs e) => e.GetPosition(TheCanvas);

    public bool SnapToGrid { get; set; } = true;

    private void TheCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed && PanPoint.HasValue)
        {
            var diff = GetWindowPoint(e) - PanPoint.Value;

            CanvasTranslateTransform.X = PanValue.X + diff.X;
            CanvasTranslateTransform.Y = PanValue.Y + diff.Y;
        }

        if (InputState == InputState.MovePoint && MoveShapes.Any())
        {
            var point = GetCanvasPoint(e);

            point = SnapPoint(point);
            foreach (var shape in MoveShapes)
            {
                if (shape is Ellipse ellipse)
                {
                    ellipse.SetCenterLocation(point);
                }
                if (shape is Line line)
                {
                    SetLineEnd(line, point);
                }
            }
        }
        else if (InputState != InputState.None)
        {
            if (TempLines.TryPeek(out var tl))
            {
                var point = GetCanvasPoint(e);

                SetLineEnd(tl, SnapPoint(point));
            }
        }
    }

    private Point SnapPoint(Point point) => SnapPoint(point, SnapToGrid && !Keyboard.IsKeyDown(Key.LeftAlt));
    private Point SnapPoint(Point point, bool snap) => snap ? SnapPoint(point, snap, SnapGridWidth) : point;
    private Point SnapPoint(Point point, bool snap, int snapGridWidth) => snap ? ((int)((point.X + snapGridWidth / 2) / snapGridWidth) * snapGridWidth, (int)((point.Y + snapGridWidth / 2) / snapGridWidth) * snapGridWidth) : point;

    // TODO put the zoom functionality into a behaviour
    private void TheCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        var pos = e.GetPosition(TheCanvas);
        CanvasScaleTransform.CenterX = pos.X;
        CanvasScaleTransform.CenterY = pos.Y;
        if (e.Delta > 0)
        {
            CanvasScaleTransform.ScaleX *= 2;
            CanvasScaleTransform.ScaleY *= 2;
        }
        else
        {
            CanvasScaleTransform.ScaleX /= 2;
            CanvasScaleTransform.ScaleY /= 2;
        }
    }

    private void TheCanvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            PanPoint = null;
        }
    }

    private void PutItemB_Click(object sender, RoutedEventArgs e)
    {
        foreach (var conveyor in AutoRoot.Conveyors)
        {
            conveyor.SpawnItems(ShapeProvider, FirstOnlyCB.IsChecked);
        }
    }

    private List<Ellipse> MoveCircles = new();

    private List<Shape> MoveShapes = new();

    private bool _IsRunning;
    public bool IsRunning
    {
        get => _IsRunning;
        set => Func.Setter(ref _IsRunning, value, isRunning => AutoRoot.Conveyors.ForEach(c => c.IsRunning = isRunning));
    }

    private void RunningCB_Click(object sender, RoutedEventArgs e) => IsRunning = RunningCB.IsChecked ?? false;

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => IsRunning = false;

    private void MovePointB_Click(object sender, RoutedEventArgs e)
    {
        RunningCB.IsChecked = false;

        foreach (var conveyor in AutoRoot.Conveyors)
        {
            foreach (var point in conveyor.Points)
            {
                var circle = ShapeProvider.CreatePointMoveCircle(point.Location, MoveCircleClicked);
                circle.Tag = point;
                TheCanvas.Children.Add(circle);
                MoveCircles.Add(circle);
            }
        }
    }

    private void MoveCircleClicked(Shape shape)
    {
        if (shape is Ellipse moveCircle && moveCircle.Tag is ConveyorPoint point)
        {
            const double size = 5d;
            var newCircle = new Ellipse()
            {
                Width = size,
                Height = size,
                Fill = Brushes.Yellow,
                Tag = point,
            };
            newCircle.SetCenterLocation(point.Location);
            TheCanvas.Children.Add(newCircle);

            MoveShapes.Add(newCircle);
            var (prev, last) = point.GetAdjacentSegments();

            if (prev is { })
            {
                var prevLine = new Line()
                {
                    X1 = prev.StartEnd.P1.X,
                    Y1 = prev.StartEnd.P1.Y,
                    X2 = prev.StartEnd.P2.X,
                    Y2 = prev.StartEnd.P2.Y,
                    Stroke = Brushes.Yellow,
                    Tag = point,
                };
                TheCanvas.Children.Add(prevLine);
                MoveShapes.Add(prevLine);
            }
            if (last is { })
            {
                var nextLine = new Line()
                {
                    X1 = last.StartEnd.P2.X,
                    Y1 = last.StartEnd.P2.Y,
                    X2 = last.StartEnd.P1.X,
                    Y2 = last.StartEnd.P1.Y,
                    Stroke = Brushes.Yellow,
                    Tag = point,
                };
                TheCanvas.Children.Add(nextLine);
                MoveShapes.Add(nextLine);
            }
        }

        foreach (var circle in MoveCircles)
        {
            TheCanvas.Children.Remove(circle);
        }
        MoveCircles.Clear();

        InputState = InputState.MovePoint;
    }

    private void SelectB_Click(object sender, RoutedEventArgs e) => SelectionManager.ToggleSelectMode();


    private async void RunB_Click(object sender, RoutedEventArgs e) => await ScriptRunner.RunScript(textEditor.Text);

    private ScriptRunner ScriptRunner = new();

    private void HappyBirthdayRubyB_Click(object sender, RoutedEventArgs e)
    {
        int xOffset;
        int yOffset = 0;
        double scaling = 5;
        foreach (var wordCoords in Func.GetTextLocations("HAPPY", "BIRTHDAY", "RUBY"))
        {
            xOffset = 0;

            foreach (var charCoords in wordCoords)
            {
                foreach (var strokecoords in charCoords)
                {
                    AutoRoot.AddConveyor(strokecoords.Scale(scaling + yOffset).Add((xOffset * 60 * (1+ (yOffset * 0.2))  + 40, yOffset * 90 + 40)), true, yOffset + 2);
                }
                xOffset++;
            }

            yOffset++;
        }
    }
}

public enum InputState
{
    None,
    SelectFirstPoint,
    SelectLastPoint,
    MovePoint,
}

public enum ActionResults
{
    Continue,
    Finish,
    Abort,
    AbortAll,
}
