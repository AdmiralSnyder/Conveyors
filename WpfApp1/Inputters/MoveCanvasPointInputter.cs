using ConveyorLib;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfLib;
using ConveyorApp.Inputters;
using UILib.Shapes;
using WpfLib.Shapes;

namespace ConveyorApp.Inputters;

public class MoveInputter : StatefulInputter<MoveInputter, Vector, MoveInputter.InputStates, CanvasInputContext>
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

    private readonly List<IEllipse> MoveCircles = new();

    private readonly List<IShape> MoveShapes = new();

    protected override void InputStateChanged(InputStates newValue)
    {
        base.InputStateChanged(newValue);
        if (newValue == InputStates.Move)
        {
            foreach (var conveyor in Context.ViewModel.AutoRoot.Conveyors)
            {
                foreach (var point in conveyor.Points)
                {
                    var circle = Context.ViewModel.ShapeProvider.CreatePointMoveCircle(point.Location, MoveCircleClicked);
                    circle.Tag = point;
                    Context.Canvas.AddToCanvas(circle);
                    MoveCircles.Add(circle);
                }
            }
        }
    }

    private void MoveCircleClicked(IShape shape)
    {
        if (shape is IEllipse moveCircle && moveCircle.Tag is ConveyorPoint point)
        {
            const double size = 5d;
            var newCircle = new WpfEllipse(new()
            {
                Width = size,
                Height = size,
                Fill = Brushes.Yellow,
                Tag = point,
            });
            newCircle.SetCenterLocation(point.Location);
            Context.Canvas.AddToCanvas(newCircle);

            MoveShapes.Add(newCircle);
            var (prev, last) = point.GetAdjacentSegments();

            if (prev is { })
            {
                var prevLine = new WpfLine(new()
                {
                    X1 = prev.StartEnd.P1.X,
                    Y1 = prev.StartEnd.P1.Y,
                    X2 = prev.StartEnd.P2.X,
                    Y2 = prev.StartEnd.P2.Y,
                    Stroke = Brushes.Yellow,
                    Tag = point,
                });
                Context.Canvas.AddToCanvas(prevLine);
                MoveShapes.Add(prevLine);
            }
            if (last is { })
            {
                var nextLine = new WpfLine(new()
                {
                    X1 = last.StartEnd.P2.X,
                    Y1 = last.StartEnd.P2.Y,
                    X2 = last.StartEnd.P1.X,
                    Y2 = last.StartEnd.P1.Y,
                    Stroke = Brushes.Yellow,
                    Tag = point,
                });
                Context.Canvas.AddToCanvas(nextLine);
                MoveShapes.Add(nextLine);
            }
        }

        foreach (var circle in MoveCircles)
        {
            Context.Canvas.RemoveFromCanvas(circle);
        }
        MoveCircles.Clear();

        InputState = InputStates.Move;
    }


    public override void HandleMouseDown(object sender, MouseButtonEventArgs e)
    {
        base.HandleMouseDown(sender, e);

        if (MoveShapes.FirstOrDefault() is { Tag: ConveyorPoint point })
        {
            Context.ViewModel.AutoRoot.MovePoint(point, Context.SnapPoint(Context.GetCanvasPoint(e)));
        }

        foreach (var shape in MoveShapes)
        {
            Context.Canvas.RemoveFromCanvas(shape);
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
                if (shape is IEllipse ellipse)
                {
                    ellipse.SetCenterLocation(point);
                }
                if (shape is ILine line)
                {
                    Context.SetLineEnd(line, point);
                }
            }
        }
    }
}
