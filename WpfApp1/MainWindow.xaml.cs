using ConveyorLib;
using CoreLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UILib;
using WpfLib;

namespace ConveyorApp;

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

        InputContext = new CanvasInputContext()
        {
            Canvas = TheCanvas,
            NotesLabel = NotesLabel,
            MainWindow = this,
        };

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

    private Inputter CurrentInputter;

    public abstract class Inputter
    {
        public virtual void HandleMouseDown(object sender, MouseButtonEventArgs e) { }

        public virtual void HandleMouseMove(object sender, MouseEventArgs e) { }

        public abstract void Start();
    }


    public abstract class Inputter<TInputter, TInputState, TContext> : Inputter
        where TInputter : Inputter<TInputter, TInputState, TContext>, new()
        where TInputState : struct, Enum
        where TContext : InputContextBase
    {
        public TContext Context { get; private set; }

        public static TInputter StartInput3(TContext context)
        {
            return new() { Context = context };
        }

        private TInputState _InputState;
        protected TInputState InputState
        {
            get => _InputState;
            set
            {
                if (!value.Equals(_InputState))
                {
                    var oldValue = _InputState;
                    _InputState = value;
                    InputStateChanged(oldValue, value);
                }
            }
        }

        protected virtual void InputStateChanged(TInputState oldValue, TInputState newValue)
        {
            InputStateChanged(newValue);
        }

        protected virtual void InputStateChanged(TInputState newValue)
        { }
    }

    public class MoveInputter : Inputter<MoveInputter, MoveInputter.InputStates, CanvasInputContext>
    {
        public enum InputStates
        {
            None,
            Move,
        }

        public override void Start() => InputState = InputStates.Move;

        private readonly List<Ellipse> MoveCircles = new();

        private readonly List<Shape> MoveShapes = new();

        protected override void InputStateChanged(InputStates newValue)
        {
            base.InputStateChanged(newValue);
            if (newValue == InputStates.Move)
            {
                foreach (var conveyor in Context.MainWindow.AutoRoot.Conveyors)
                {
                    foreach (var point in conveyor.Points)
                    {
                        var circle = Context.MainWindow.ShapeProvider.CreatePointMoveCircle(point.Location, MoveCircleClicked);
                        circle.Tag = point;
                        Context.Canvas.Children.Add(circle);
                        MoveCircles.Add(circle);
                    }
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
                Context.Canvas.Children.Add(newCircle);

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
                    Context.Canvas.Children.Add(prevLine);
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
                    Context.Canvas.Children.Add(nextLine);
                    MoveShapes.Add(nextLine);
                }
            }

            foreach (var circle in MoveCircles)
            {
                Context.Canvas.Children.Remove(circle);
            }
            MoveCircles.Clear();

            InputState = InputStates.Move;
        }


        public override void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            base.HandleMouseDown(sender, e);

            if (MoveShapes.FirstOrDefault() is { Tag: ConveyorPoint point })
            {
                Context.MainWindow.AutoRoot.MovePoint(point, Context.SnapPoint(Context.GetCanvasPoint(e)));
            }

            foreach (var shape in MoveShapes)
            {
                Context.Canvas.Children.Remove(shape);
            }
            MoveShapes.Clear();
        }

        public override void HandleMouseMove(object sender, MouseEventArgs e)
        {
            base.HandleMouseMove(sender, e);

            if (MoveShapes.Any())
            {
                var point = Context.GetCanvasPoint(e);

                point = Context.SnapPoint(point);
                foreach (var shape in MoveShapes)
                {
                    if (shape is Ellipse ellipse)
                    {
                        ellipse.SetCenterLocation(point);
                    }
                    if (shape is Line line)
                    {
                        Context.SetLineEnd(line, point);
                    }
                }
            }
        }
    }

    public class ConveyorInputter : Inputter<ConveyorInputter, ConveyorInputter.InputStates, CanvasInputContext>
    {
        public enum InputStates
        {
            None,
            SelectFirstPoint,
            SelectLastPoint,
        }
        public override void Start() => InputState = InputStates.SelectFirstPoint;

        protected override void InputStateChanged(InputStates newValue)
        {
            base.InputStateChanged(newValue);

            switch (newValue)
            {
                case InputStates.SelectFirstPoint:
                    Context.SetCursor(Cursors.Cross);
                    Context.UserNotes = "Please select the starting point.";
                    Context.CaptureMouse();
                    break;

                case InputStates.SelectLastPoint:
                    Context.SetCursor(Cursors.Cross);
                    Context.UserNotes = "Please select the ending point.";
                    break;

                case InputStates.None:
                    Context.SetCursor(Cursors.Arrow);
                    Context.ReleaseMouseCapture();
                    Context.UserNotes = "Click around. Have fun!";
                    break;
            }
        }

        public override void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            base.HandleMouseDown(sender, e);

            var modifier = Keyboard.Modifiers;
            var isShiftPressed = modifier.HasFlag(ModifierKeys.Shift);
            switch (InputState)
            {
                case InputStates.SelectFirstPoint:
                    switch (AddPoint())
                    {
                        case ActionResults.Continue:
                        case ActionResults.Finish:
                            InputState = InputStates.SelectLastPoint;
                            break;
                        case ActionResults.Abort:
                        case ActionResults.AbortAll:
                            AbortAll();
                            break;
                    }
                    break;
                case InputStates.SelectLastPoint:
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
            }

            void Abort(bool abortAllIfEmpty = false)
            {
                if (TempLines.TryPop(out var last))
                {
                    Context.Canvas.Children.Remove(last);
                }
                if (TempLines.TryPeek(out last))
                {
                    Context.SetLineEnd(last, Context.SnapPoint(Context.GetCanvasPoint(e)));
                }
                else
                {
                    if (abortAllIfEmpty)
                    {
                        InputState = InputStates.None;
                    }
                }
            }

            void AbortAll()
            {
                foreach (var line in TempLines)
                {
                    Context.Canvas.Children.Remove(line);
                }
                TempLines.Clear();
                InputState = InputStates.None;
            }

            ActionResults AddPoint()
            {
                if (e.ChangedButton == MouseButton.Right) return isShiftPressed
                    ? ActionResults.AbortAll
                    : ActionResults.Abort;

                var point = Context.SnapPoint(Context.GetCanvasPoint(e));
                TempLines.Push(Context.AddLine(point, point)); // das ist geschummelt, damit ich nicht umständlich Zustände speichern muss

                return isShiftPressed ? ActionResults.Continue : ActionResults.Finish;
            }

            void Finish()
            {
                Point lastPoint = (double.NaN, double.NaN);
                List<Point> points = new();
                foreach (var line in TempLines.Reverse())
                {
                    Context.Canvas.Children.Remove(line);
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

                InputState = InputStates.None;

                Context.MainWindow.AutoRoot.AddConveyor(points, Context.MainWindow.IsRunning, int.TryParse(Context.MainWindow.LanesCountTB.Text, out var lanesCnt) ? Math.Max(lanesCnt, 1) : 1);
            }
        }

        private readonly Stack<Line> TempLines = new();

        public override void HandleMouseMove(object sender, MouseEventArgs e)
        {
            base.HandleMouseMove(sender, e);

            if (InputState != InputStates.None)
            {
                if (TempLines.TryPeek(out var tl))
                {
                    var point = Context.GetCanvasPoint(e);

                    Context.SetLineEnd(tl, Context.SnapPoint(point));
                }
            }
        }
    }

    public class CanvasInputContext : InputContextBase
    {
        public Canvas Canvas { get; set; }
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
            if (NotesLabel is { })
            {
                NotesLabel.Text = UserNotes;
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
        public int SnapGridWidth { get; set; }
        public bool SnapToGrid { get; set; }

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
            var line = MainWindow.ShapeProvider.CreateConveyorPositioningLine(((Point)from, (Point)to));
            Canvas.Children.Add(line);
            return line;
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

        }

    }

    private CanvasInputContext InputContext;

    public abstract class InputContextBase
    {
        public MainWindow MainWindow { get; set; }

        public abstract void SetCursor(Cursor cursor);

        private string? _UserNotes;
        public string UserNotes
        {
            get => _UserNotes;
            set
            {
                _UserNotes = value;
                UserNotesChanged();
            }
        }

        public virtual void CaptureMouse() { }
        public virtual void ReleaseMouseCapture() { }

        protected virtual void UserNotesChanged() { }

        internal void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (HandleMouseDownPanning(e)) return;

            if (MainWindow.CurrentInputter is { } ci)
            {
                ci.HandleMouseDown(sender, e);
            }

            HandleMouseDownVirtual(e);
        }

        protected virtual void HandleMouseDownVirtual(MouseButtonEventArgs e) { }

        protected abstract bool HandleMouseDownPanning(MouseButtonEventArgs e);

        protected abstract bool HandleMouseMovePanning(MouseEventArgs e);

        internal void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (HandleMouseMovePanning(e)) return;

            MouseMovedInCanvas?.Invoke(sender, e);

            if (MainWindow.CurrentInputter is { } ci)
            {
                ci.HandleMouseMove(sender, e);
            }
        }

        public event MouseEventHandler MouseMovedInCanvas;

        //public event MouseButtonEventHandler LeftMouseButtonClicked;
        //public event MouseButtonEventHandler RightMouseButtonClicked;
        //public event MouseButtonEventHandler MouseWheelClicked;

        public event EventHandler<EventArgs<Point>> LeftMouseButtonClicked;
        protected void DoLeftMouseButtonClicked(Point point)
        {
            LeftMouseButtonClicked?.Invoke(this, new(point));
        }

        internal void HandleMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (HandleMouseUpPanning(e)) return;
        }

        protected abstract bool HandleMouseUpPanning(MouseButtonEventArgs e);

    }

    public class CircleInputter1 : Inputter<CircleInputter1, CircleInputter1.InputStates, CanvasInputContext>
    {
        public enum InputStates
        {
            SelectCenterPoint,
            SelectCircumferencePoint,
        }
        public override void Start() => InputState = InputStates.SelectCenterPoint;
    }

    public class PointInputter : Inputter<PointInputter, PointInputter.InputStates, CanvasInputContext>
    {
        public enum InputStates
        {
            None,
            SelectPoint,
        }

        public Point Result { get; set; }

        public override void Start()
        {
            Context.LeftMouseButtonClicked += Context_LeftMouseButtonClicked;
            InputState = InputStates.SelectPoint;
        }

        public void Complete()
        {
            Context.LeftMouseButtonClicked -= Context_LeftMouseButtonClicked;
            TaskCompletionSource.SetResult(Result);
        }

        private void Context_LeftMouseButtonClicked(object? sender, EventArgs<Vector> e)
        {
            Result = e.Data;
            Complete();
        }

        private TaskCompletionSource<Point?> TaskCompletionSource;

        public Task<Point?> StartAsync()
        {
            Start();
            TaskCompletionSource = new();
            return TaskCompletionSource.Task;
        }

        public static Task<Point?> StartInput(CanvasInputContext context)
        {
            var inputter = StartInput3(context);
            return inputter.StartAsync();
        }
    }



    private async void AddPointB_Click(object sender, RoutedEventArgs e)
    {
        if (await PointInputter.StartInput(this.InputContext) is { } point)
        {
            AddPoint(point);
        }
    }

    private Shape AddPoint(Point point)
    {
        var pointShape = ShapeProvider.CreatePoint(point);
        TheCanvas.Children.Add(pointShape);
        return pointShape;
    }

    private void AddConveyorB_Click(object sender, RoutedEventArgs e)
    {
        (CurrentInputter = ConveyorInputter.StartInput3(this.InputContext)).Start();
    }

    private TInputter AddInputter<TInputter>(TInputter inputter) where TInputter : Inputter
    {
        CurrentInputter = inputter;
        return inputter;
    }


    private void AddCircleB_Click(object sender, RoutedEventArgs e)
    {
        (CurrentInputter = CircleInputter1.StartInput3(this.InputContext)).Start();
    }

    //private InputState _InputState;
    //private InputState InputState
    //{
    //    get => _InputState;
    //    set
    //    {
    //        _InputState = value;
    //        switch (_InputState)
    //        {
    //            case InputState.SelectFirstPoint:
    //                TheCanvas.Cursor = Cursors.Cross;
    //                Mouse.Capture(TheCanvas);
    //                NotesLabel.Text = "Please select the starting point.";
    //                break;
    //            case InputState.SelectLastPoint:
    //                TheCanvas.Cursor = Cursors.Cross;
    //                NotesLabel.Text = "Please select the ending point.";
    //                break;
    //            case InputState.None:
    //                TheCanvas.Cursor = Cursors.Arrow;
    //                Mouse.Capture(null);
    //                NotesLabel.Text = "Click around. Have fun!";
    //                break;
    //        }
    //    }
    //}

    private void TheCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        InputContext.HandleMouseDown(sender, e);
    }

    //private void OnPropertyChanged(string propertyName)
    //{
    //    if (this.PropertyChanged != null)
    //    {
    //        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}


    private ConveyorShapeProvider ShapeProvider;

    private void TheCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        InputContext.HandleMouseMove(sender, e);
    }
    

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

        InputContext.HandleMouseUp(sender, e);
    }

    private void PutItemB_Click(object sender, RoutedEventArgs e)
    {
        foreach (var conveyor in AutoRoot.Conveyors)
        {
            conveyor.SpawnItems(ShapeProvider, FirstOnlyCB.IsChecked);
        }
    }

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
        (CurrentInputter = MoveInputter.StartInput3(this.InputContext)).Start();
    }

    

    private void SelectB_Click(object sender, RoutedEventArgs e) => SelectionManager.ToggleSelectMode();


    private async void RunB_Click(object sender, RoutedEventArgs e) => await ScriptRunner.RunScript(textEditor.Text);

    private ScriptRunner ScriptRunner = new();

    private void HappyBirthdayRubyB_Click(object sender, RoutedEventArgs e) => WriteString("R");
    //WriteString("""
    //HAPPY
    //BIRTHDAY
    //RUBY
    //""");

    private void WriteString(string text) => WriteStrings(text.Split(Environment.NewLine));

    private void WriteStrings(IEnumerable<string> lines)
    {
        int xOffset;
        int yOffset = 0;
        double scaling = 5;
        foreach (var wordCoords in Func.GetTextLocations(lines))
        {
            xOffset = 0;

            foreach (var charCoords in wordCoords)
            {
                foreach (var strokecoords in charCoords)
                {
                    AutoRoot.AddConveyor(strokecoords.Scale(scaling + yOffset).Add((xOffset * 60 * (1 + (yOffset * 0.2)) + 40, yOffset * 90 + 40)), true, yOffset + 2);
                }
                xOffset++;
            }

            yOffset++;
        }
    }
}

public enum ActionResults
{
    Continue,
    Finish,
    Abort,
    AbortAll,
}
