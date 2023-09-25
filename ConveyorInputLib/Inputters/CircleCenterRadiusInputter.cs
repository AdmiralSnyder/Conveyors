using System.Threading.Tasks;
using ConveyorInputLib.Helpers;
using InputLib;
using InputLib.Inputters;

namespace ConveyorInputLib.Inputters;

public class CircleCenterRadiusInputter : Inputter<CircleCenterRadiusInputter, (Point Center, double Radius), CanvasInputHelpers>
{
    protected override async Task<InputResult<(Vector Center, double Radius)>> StartAsyncVirtual()
        => await InputManager.Blank()
            .Then(async _ => await PointInputter.StartInput(Context,
                Helpers.ShowUserNotes("Please select the starting point"),
                Helpers.ShowMouseLocation()))
            .Then(async ctx => await PointInputter.StartInput(Context,
                Helpers.ShowUserNotes("Please select a point on the circle"),
                Helpers.ShowMouseLocation(),
                Helpers.FixedPoint(ctx.Second),
                Helpers.LineFromToMouse(ctx.Second),
                Helpers.ShowCircleByRadius(ctx.Second)))
            .Do(ctx => InputResult.SuccessTask((ctx.First.Second, (ctx.Second - ctx.First.Second).Length())));
}
