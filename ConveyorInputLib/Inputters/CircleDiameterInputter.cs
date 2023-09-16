using System.Threading.Tasks;
using ConveyorInputLib.Helpers;
using InputLib;

namespace ConveyorInputLib.Inputters;

public class CircleDiameterInputter : Inputter<CircleDiameterInputter, (Point Point1, Point Point2), CanvasInputHelpers>
{
    protected override async Task<InputResult<(Vector Point1, Vector Point2)>> StartAsyncVirtual()
        => await InputManager.Blank()
        .Then(async _ => await PointInputter.StartInput(Context,
            Helpers.ShowMouseLocation()))
        .Then(async ctx => await PointInputter.StartInput(Context,
            Helpers.ShowMouseLocation(),
            Helpers.FixedPoint(ctx.Second),
            Helpers.ShowCircleByDiameter(ctx.Second),
            Helpers.ShowCalculatedPoint(mouse => Maths.GetMidPoint(ctx.Second, mouse))))
        .Do(ctx => InputResult.SuccessTask(ctx.Flatten()));   
}
