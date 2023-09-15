﻿using System;
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
        Context.CurrentInputter = this;
    }

    protected override void InputStateChanged(InputStates newValue)
    {
        base.InputStateChanged(newValue);

        switch (newValue)
        {
            case InputStates.SelectFirstPoint:
                Context.SetCursor(InputCursors.Cross);
                Context.UserNotes = "Please select the starting point.";
                Context.CaptureMouse();
                break;

            case InputStates.SelectLastPoint:
                Context.SetCursor(InputCursors.Cross);
                Context.UserNotes = "Please select the ending point.";
                break;

            case InputStates.None:
                Context.SetCursor(InputCursors.Arrow);
                Context.ReleaseMouseCapture();
                Context.UserNotes = "Click around. Have fun!";
                break;
        }
    }

    public override void HandleMouseDown(object sender, EventArgs e)
    {
        base.HandleMouseDown(sender, e);

        var isShiftPressed = Context.IsShiftPressed();
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
                Context.RemoveTempShape(last);
            }
            if (TempLines.TryPeek(out last))
            {
                last.SetEnd(Context.GetPoint(e));
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
                Context.RemoveTempShape(line);
            }
            TempLines.Clear();
            InputState = InputStates.None;
            this.Abort();
        }

        ActionResults AddPoint()
        {
            if (Context.IsRightClick(e)) return isShiftPressed
                ? ActionResults.AbortAll
                : ActionResults.Abort;

            var point = Context.GetPoint(e);

            var line = ShapeProvider.CreateConveyorPositioningLine((point, point));  // das ist geschummelt, damit ich nicht umständlich Zustände speichern muss
            Context.AddTempShape(line);
            TempLines.Push(line);

            return isShiftPressed ? ActionResults.Continue : ActionResults.Finish;
        }

        void Finish()
        {
            List<Point> points = new();
            Point lastPoint = (double.NaN, double.NaN);
            foreach (var line in TempLines.Reverse())
            {
                Context.RemoveTempShape(line);
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
            tl.SetEnd(Context.GetPoint(e));
        }
    }
}
