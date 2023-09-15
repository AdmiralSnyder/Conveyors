using System.Collections.Generic;
using UILib.Shapes;
using ConveyorLib.Objects.Conveyor;
using InputLib;

namespace ConveyorInputLib;

public abstract class MoveCanvasPointInputterBase<TInputter, TPoint> : StatefulInputter<TInputter, (TPoint, Point), MoveCanvasPointInputterBase<TInputter, TPoint>.InputStates>
    where TInputter : MoveCanvasPointInputterBase<TInputter, TPoint>, new()
    where TPoint : IMovable
{
    public enum InputStates
    {
        None,
        Move,
    }

    public override void Start()
    {
        InputState = InputStates.Move;
        Context.CurrentInputter = this;
    }

    protected readonly List<IEllipse> MoveCircles = new();

    protected readonly List<IShape> MoveShapes = new();

    protected override void InputStateChanged(InputStates newValue)
    {
        if (newValue == InputStates.Move)
        {
            foreach (var conveyor in AutoRoot.Conveyors)
            {
                foreach (var point in conveyor.Points)
                {
                    var circle = ShapeProvider.CreatePointMoveCircle(point.Location, MoveCircleClicked);
                    circle.Tag = point;
                    Context.AddTempShape(circle);
                    MoveCircles.Add(circle);
                }
            }
        }
        base.InputStateChanged(newValue);
    }

    public override IEnumerable<IShape> GetMouseDownShapes() => MoveCircles;

    private void MoveCircleClicked(IShape shape)
    {
        if (shape is IEllipse moveCircle && moveCircle.Tag is ConveyorPoint point)
        {
            const double size = 5d;
            var newCircle = ShapeProvider.CreateCircle(point.Location, size / 2);
            newCircle.Tag = point;
            newCircle.FillColor = System.Drawing.Color.Yellow;
            Context.AddTempShape(newCircle);

            MoveShapes.Add(newCircle);
            var (prev, last) = point.GetAdjacentSegments();

            if (prev is { })
            {
                var prevLine = ShapeProvider.CreateLineSegment(prev.StartEnd);
                prevLine.StrokeColor = System.Drawing.Color.Yellow;
                prevLine.Tag = point;
                Context.AddTempShape(prevLine);
                MoveShapes.Add(prevLine);
            }
            if (last is { })
            {
                var nextLine = ShapeProvider.CreateLineSegment((last.StartEnd.P2, last.StartEnd.P1));
                nextLine.StrokeColor = System.Drawing.Color.Yellow;
                nextLine.Tag = point;
                Context.AddTempShape(nextLine);
                MoveShapes.Add(nextLine);
            }

            Result = ((TPoint)(object)point, default);
        }


        foreach (var circle in MoveCircles)
        {
            Context.RemoveTempShape(circle);
        }
        MoveCircles.Clear();

        InputState = InputStates.Move;
    }
}
