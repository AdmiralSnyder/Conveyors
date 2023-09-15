using ConveyorApp.Inputters.Helpers;
using CoreLib;
using InputLib;
using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

/// <summary>
/// lets the user input the three points that will be used to create a circle
/// </summary>
class CircleThreePointsInputter : Inputter<CircleThreePointsInputter, (Point Point1, Point Point2, Point Point3), CanvasInputHelpers>
{
    protected override async Task<InputResult<(Point Point1, Point Point2, Point Point3)>> StartAsyncVirtual() 
    => await InputManager.Blank()
        .Then(async _ => await PointInputter.StartInput(Context,
            Helpers.ShowMouseLocation()))
        .Then(async ctx => await PointInputter.StartInput(Context, 
            Helpers.ShowMouseLocation(),
            Helpers.FixedPoint(ctx.Second)))
        .Then(async ctx => await PointInputter.StartInput(Context,
            Helpers.ShowMouseLocation(),
            Helpers.FixedPoint(ctx.First.Second),
            Helpers.FixedPoint(ctx.Second),
            Helpers.ShowThreePointCircleOnMouseLocation(ctx.First.Second, ctx.Second)))
        .Do(ctx => InputResult.SuccessTask(ctx.Flatten()));
}
