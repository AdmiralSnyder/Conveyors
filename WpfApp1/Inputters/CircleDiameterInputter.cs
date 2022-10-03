using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

class CircleDiameterInputter : Inputter<CircleDiameterInputter, (Point Point1, Point Point2), CanvasInputContext, CanvasInputHelpers>
{
    public override async Task<InputResult<(Vector Point1, Vector Point2)>> StartAsyncVirtual()
        => await InputManager.Blank()
        .Then(async _ => await PointInputter.StartInput(Context,
            Helpers.ShowMouseLocation()))
        .Then(async ctx => await PointInputter.StartInput(Context,
            Helpers.ShowMouseLocation(),
            Helpers.FixedPoint(ctx.Second),
            Helpers.ShowCalculatedPoint(mouse => Maths.GetMidPoint(ctx.Second, mouse))))
        .Do(ctx => InputResult.SuccessTask(ctx.Flatten()));

    
}
