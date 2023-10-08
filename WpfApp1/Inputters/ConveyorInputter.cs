using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using InputLib;
using UILib.Shapes;

namespace ConveyorApp.Inputters;

public class ConveyorInputter : StatefulInputter<ConveyorInputter, IEnumerable<Point>, ConveyorInputter.InputStates>
{
    public enum InputStates
    {
        None,
        SelectFirstPoint,
        SelectLastPoint,
    }

    public override void Start()
    {
        base.Start();
        InputState = InputStates.SelectFirstPoint;
        InputContext.CurrentInputter = this;
    }

    protected override void InputStateChanged(InputStates newValue)
    {
        base.InputStateChanged(newValue);

        switch (newValue)
        {
            case InputStates.SelectFirstPoint:
                InputContext.SetCursor(InputCursors.Cross);
                InputContext.UserNotes = "Please select the starting point.";
                InputContext.CaptureMouse();
                break;

            case InputStates.SelectLastPoint:
                InputContext.SetCursor(InputCursors.Cross);
                InputContext.UserNotes = "Please select the ending point.";
                break;

            case InputStates.None:
                InputContext.SetCursor(InputCursors.Arrow);
                InputContext.ReleaseMouseCapture();
                InputContext.UserNotes = "Click around. Have fun!";
                break;
        }
    }

    public override void HandleMouseDown(object sender, EventArgs e)
    {
        base.HandleMouseDown(sender, e);

        var isShiftPressed = InputContext.IsShiftPressed();
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
                        CancelAll();
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
                        Cancel(true);
                        break;
                    case ActionResults.AbortAll:
                        CancelAll();
                        break;
                }
                break;
        }

        void Cancel(bool cancelAllIfEmpty = false)
        {
            if (TempLines.TryPop(out var last))
            {
                InputContext.RemoveTempShape(last);
            }
            if (TempLines.TryPeek(out last))
            {
                last.SetEnd(InputContext.GetPoint(e));
            }
            else
            {
                if (cancelAllIfEmpty)
                {
                    InputState = InputStates.None;
                    this.Abort();
                }
            }
        }

        void CancelAll()
        {
            foreach (var line in TempLines)
            {
                InputContext.RemoveTempShape(line);
            }
            TempLines.Clear();
            InputState = InputStates.None;
            this.Abort();
        }

        ActionResults AddPoint()
        {
            if (InputContext.IsRightClick(e)) return isShiftPressed
                ? ActionResults.AbortAll
                : ActionResults.Abort;

            var point = InputContext.GetPoint(e);

            var line = ShapeProvider.CreateConveyorPositioningLine((point, point));  // das ist geschummelt, damit ich nicht umständlich Zustände speichern muss
            InputContext.AddTempShape(line);
            TempLines.Push(line);

            return isShiftPressed ? ActionResults.Continue : ActionResults.Finish;
        }

        void Finish()
        {
            List<Point> points = new();
            Point lastPoint = (double.NaN, double.NaN);
            foreach (var line in TempLines.Reverse())
            {
                InputContext.RemoveTempShape(line);
                line.StrokeColor = System.Drawing.Color.Red;
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
            Complete(points);
        }
    }

    private readonly Stack<ILine> TempLines = new();

    public override void HandleMouseMove(object sender, EventArgs e)
    {
        base.HandleMouseMove(sender, e);

        if (InputState != InputStates.None && TempLines.TryPeek(out var tl))
        {
            tl.SetEnd(InputContext.GetPoint(e));
        }
    }
}
