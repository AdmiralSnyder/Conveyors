using System.Linq;
using UILib.Shapes;
using ConveyorLib.Objects.Conveyor;
using UILib;
using System.Diagnostics;

namespace ConveyorInputLib.Inputters;

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
        Context.Notify();
    }
    static int moveCount = 0;
    public override void HandleMouseMove(object sender, EventArgs e)
    {
        
        moveCount++;
        base.HandleMouseMove(sender, e);

        var point = Context.GetPoint(e);
        //var sw = Stopwatch.StartNew();
        var match = MoveCircles.Any(mc => mc.GetCircleDefinition(out var cd) && Maths.PointIsInCircle(point, cd));
        //sw.Stop();
        if (match)
        {

            Context.SetCursor(InputLib.InputCursors.Hand);
        }
        else
        {
            Context.SetCursor(InputLib.InputCursors.Arrow);
        }

        if (MoveShapes.Any())
        {
            foreach (var shape in MoveShapes)
            {
                if (shape is IEllipse ellipse)
                {
                    ellipse.SetCenterLocation(point);
                    Context.NotifyTemp();
                }
                if (shape is ILine line)
                {
                    line.SetEnd(point);
                    Context.NotifyTemp();
                }
            }
        }
    }
}
