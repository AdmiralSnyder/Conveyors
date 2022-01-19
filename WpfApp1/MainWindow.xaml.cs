using System;
using System.Collections.Generic;
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

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void AddConveyorB_Click(object sender, RoutedEventArgs e)
    {
        InputState = InputState.SelectFirstPoint;
    }
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
        var isShiftPressed = modifier == ModifierKeys.Shift;
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
        }

        void Abort(bool abortAllIfEmpty = false)
        {
            if (TempLines.TryPop(out var last))
            {
                TheCanvas.Children.Remove(last);
            }
            if (TempLines.TryPeek(out last))
            {
                SetLineEnd(last, GetCanvasPoint(e));
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

            var point = GetCanvasPoint(e);
            TempLines.Push(AddLine(point, point)); // das ist geschummelt, damit ich nicht umständlich Zustände speichern muss

            return isShiftPressed ? ActionResults.Continue : ActionResults.Finish;
        }

        void Finish()
        {
            foreach (var line in TempLines)
            {
                TheCanvas.Children.Remove(line);
                line.Stroke = Brushes.Red;
            }
            TempLines.Pop();
            var items = new Stack<Line>(TempLines);
            var conv = Conveyor.Create(items);
            Conveyor.AddToCanvas(conv, TheCanvas);
            Conveyors.Add(conv);

            TempLines.Clear();

            InputState = InputState.None;
        }
    }

    public List<Conveyor> Conveyors = new();

    public Line AddLine(Point from, Point to)
    {
        var line = new Line() { X1 = from.X, X2 = to.X, Y1 = from.Y, Y2 = to.Y, Stroke = Brushes.Black, StrokeThickness = 2 };
        TheCanvas.Children.Add(line);
        return line;
    }

    public void AddBlock(Point point)
    {
        var block = new Rectangle() { Width = 100, Height = 100, Fill = Brushes.Black };
        Canvas.SetLeft(block, point.X);
        Canvas.SetTop(block, point.Y);
        TheCanvas.Children.Add(block);
    }

    private void SetLineEnd(Line line, Point point)
    {
        line.X2 = point.X;
        line.Y2 = point.Y;
    }

    private Point GetWindowPoint(MouseEventArgs e) => e.GetPosition(this);

    private Point GetCanvasPoint(MouseEventArgs e) => e.GetPosition(TheCanvas);

    private void TheCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed && PanPoint.HasValue)
        {
            var diff = GetWindowPoint(e) - PanPoint.Value;

            CanvasTranslateTransform.X = PanValue.X + diff.X;
            CanvasTranslateTransform.Y = PanValue.Y + diff.Y;
        }

        if (InputState != InputState.None)
        {
            if (TempLines.TryPeek(out var tl))
            {
                SetLineEnd(tl, GetCanvasPoint(e));
            }
        }
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
        if (e.ChangedButton == MouseButton.Middle)
        {
            PanPoint = null;
        }
    }

    private void PutItemB_Click(object sender, RoutedEventArgs e)
    {
        foreach (var conveyor in Conveyors)
        {
            conveyor.SpawnItem();
        }
    }

    private bool _IsRunning;
    public bool IsRunning
    {
        get => _IsRunning;
        set => Func.Setter(ref _IsRunning, value, isRunning => Conveyors.ForEach(c => c.IsRunning = isRunning));
    }

    private void RunningCB_Click(object sender, RoutedEventArgs e) => IsRunning = RunningCB.IsChecked ?? false;

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => IsRunning = false;
}

public enum InputState
{
    None,
    SelectFirstPoint,
    SelectLastPoint,
}

public enum ActionResults
{
    Continue,
    Finish,
    Abort,
    AbortAll,
}
