using ConveyorInputLib.Helpers;
using CoreLib;
using InputLib;
using InputLib.Inputters;
using System.Threading.Tasks;

namespace ConveyorInputLib.Inputters;

/// <summary>
/// lets the user input the three points that will be used to create a circle
/// </summary>
public class CircleThreePointsInputter : Inputter<CircleThreePointsInputter, (Point Point1, Point Point2, Point Point3), CanvasInputHelpers>
{
    protected override async Task<InputResult<(Point Point1, Point Point2, Point Point3)>> StartAsyncVirtual()
    => await InputManager.BlankContext()
        .Then(async _ => await PointInputter.StartInput(InputContext,
            Helpers.ShowMouseLocation()))
        .Then(async ctx => await PointInputter.StartInput(InputContext,
            Helpers.ShowMouseLocation(),
            Helpers.ShowFixedPoint(ctx.Last)))
        .Then(async ctx => await PointInputter.StartInput(InputContext,
            Helpers.ShowMouseLocation(),
            Helpers.ShowFixedPoint(ctx.Previous.Last),
            Helpers.ShowFixedPoint(ctx.Last),
            Helpers.ShowThreePointCircleOnMouseLocation(ctx.Previous.Last, ctx.Last)))
        .Do(ctx => InputResult.SuccessTask(ctx.Flatten()));
}
