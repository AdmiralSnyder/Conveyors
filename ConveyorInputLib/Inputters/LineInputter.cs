using System.Threading.Tasks;
using ConveyorInputLib.Helpers;
using ConveyorInputLib.Inputters;
using InputLib;

namespace ConveyorApp.Inputters;

public class LineInputter : Inputter<LineInputter, (Point P1, Point P2), CanvasInputHelpers>
{
    protected override async Task<InputResult<(Vector, Vector)>> StartAsyncVirtual() 
        => await InputManager.Blank()
            .Then(async _ => await PointInputter.StartInput(Context,
                Helpers.ShowMouseLocation()))
            .Then(async ctx => await PointInputter.StartInput(Context,
                Helpers.ShowMouseLocation(),
                Helpers.LineFromToMouse(ctx.Second),
                Helpers.FixedPoint(ctx.Second)))
            .Do(ctx => InputResult.SuccessTask((ctx.First.Second, ctx.Second)));
}
