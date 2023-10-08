using System.Threading.Tasks;
using ConveyorInputLib.Helpers;
using InputLib;
using InputLib.Inputters;

namespace ConveyorInputLib.Inputters;

public class CircleCenterRadiusInputter : Inputter<CircleCenterRadiusInputter, (Point Center, double Radius), CanvasInputHelpers>
{
    protected override async Task<InputResult<(Vector Center, double Radius)>> StartAsyncVirtual()
        => await InputManager.BlankContext()
            .Then(async _ => await PointInputter.StartInput(InputContext,
                Helpers.ShowUserNotes("Please select the starting point"),
                Helpers.ShowMouseLocation()))
            .Then(async ctx => await PointInputter.StartInput(InputContext,
                Helpers.ShowUserNotes("Please select a point on the circle"),
                Helpers.ShowMouseLocation(),
                Helpers.ShowFixedPoint(ctx.Last),
                Helpers.LineFromToMouse(ctx.Last),
                Helpers.ShowCircleByRadius(ctx.Last)))
            .Do(ctx => InputResult.SuccessTask((ctx.Previous.Last, (ctx.Last - ctx.Previous.Last).Length())));
}
