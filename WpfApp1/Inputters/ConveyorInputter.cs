using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ConveyorApp.Inputters;

public class ConveyorInputter : Inputter<ConveyorInputter, IEnumerable<Point>, ConveyorInputter.InputStates, CanvasInputContext>
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
