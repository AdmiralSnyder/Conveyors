using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

public class CircleCenterRadiusInputter : Inputter<CircleCenterRadiusInputter, (Point Center, double Radius), CanvasInputContext, CanvasInputHelpers>
{
    public override async Task<InputResult<(Vector Center, double Radius)>> StartAsyncVirtual() 
        => await InputManager.Blank()
            .Then(async _ => await PointInputter.StartInput(Context, 
                Helpers.ShowMouseLocation()))
            .Then(async ctx => await PointInputter.StartInput(Context, 
                Helpers.ShowMouseLocation(),
                Helpers.LineFromToMouse(ctx.Second),
                Helpers.FixedPoint(ctx.Second)))
            .Do(ctx => InputResult.SuccessTask((ctx.First.Second, (ctx.Second - ctx.First.Second).Length())));
}
