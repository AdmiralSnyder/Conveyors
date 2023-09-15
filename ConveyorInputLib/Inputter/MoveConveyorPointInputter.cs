using System.Linq;
using UILib.Shapes;
using ConveyorLib.Objects.Conveyor;
using UILib;
namespace ConveyorInputLib;

public class MoveConveyorPointInputter : MoveCanvasPointInputterBase<MoveConveyorPointInputter, ConveyorPoint>
{
    public static int Count = 0;
    public MoveConveyorPointInputter() => Count++;

    public override void HandleMouseDown(object sender, EventArgs e)
    {
        base.HandleMouseDown(sender, e);

        if (MoveShapes.FirstOrDefault() is { Tag: ConveyorPoint point })
        {
            Result = (Result.Item1, Context.GetPoint(e));
            Complete();
        }
    }

    protected override void CleanupVirtual()
    {
        base.CleanupVirtual();
        foreach (var shape in MoveShapes)
        {
            Context.RemoveTempShape(shape);
        }
        MoveShapes.Clear();
    }

    public override void HandleMouseMove(object sender, EventArgs e)
    {
        base.HandleMouseMove(sender, e);

        if (MoveShapes.Any())
        {
            var point = Context.GetPoint(e);
            foreach (var shape in MoveShapes)
            {
                if (shape is IEllipse ellipse)
                {
                    ellipse.SetCenterLocation(point);
                }
                if (shape is ILine line)
                {
                    line.SetEnd(point);
                }
            }
        }
    }
}
