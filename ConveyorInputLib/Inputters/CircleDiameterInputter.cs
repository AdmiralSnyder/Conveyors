using System.Threading.Tasks;
using ConveyorInputLib.Helpers;
using InputLib;
using InputLib.Inputters;

namespace ConveyorInputLib.Inputters;

public class CircleDiameterInputter : Inputter<CircleDiameterInputter, (Point Point1, Point Point2), CanvasInputHelpers>
{
    protected override async Task<InputResult<(Vector Point1, Vector Point2)>> StartAsyncVirtual()
        => await InputManager.BlankContext()
        .Then(async _ => await PointInputter.StartInput(InputContext,
            Helpers.ShowMouseLocation()))
        .Then(async ctx => await PointInputter.StartInput(InputContext,
            Helpers.ShowMouseLocation(),
            Helpers.ShowFixedPoint(ctx.Last),
            Helpers.ShowCircleByDiameter(ctx.Last),
            Helpers.ShowCalculatedPoint(mouse => Maths.GetMidPoint(ctx.Last, mouse))))
        .Do(ctx => InputResult.SuccessTask(ctx.Flatten()));   
}
