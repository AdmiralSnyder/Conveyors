using System.Threading.Tasks;
using ConveyorInputLib.Helpers;
using ConveyorInputLib.Inputters;
using InputLib;
using InputLib.Inputters;

namespace ConveyorApp.Inputters;

public class LineInputter : Inputter<LineInputter, (Point P1, Point P2), CanvasInputHelpers>
{
    protected override async Task<InputResult<(Vector, Vector)>> StartAsyncVirtual() 
        => await InputManager.BlankContext()
            .Then(async _ => await PointInputter.StartInput(InputContext,
                Helpers.ShowMouseLocation()))
            .Then(async ctx => await PointInputter.StartInput(InputContext,
                Helpers.ShowMouseLocation(),
                Helpers.LineFromToMouse(ctx.Last),
                Helpers.ShowFixedPoint(ctx.Last)))
            .Do(ctx => InputResult.SuccessTask((ctx.Previous.Last, ctx.Last)));
}
