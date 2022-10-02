using CoreLib;
using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

/// <summary>
/// lets the user input the three points that will be used to create a circle
/// </summary>
class CircleInputter2 : Inputter<CircleInputter2, (Point Point1, Point Point2, Point Point3), CanvasInputContext>
{
    public override async Task<InputResult<(Point Point1, Point Point2, Point Point3)>> StartAsyncVirtual() 
    => await InputManager.Blank()
        .Then(async _ => await PointInputter.StartInput(Context, 
            ShowMouseLocationInputHelper.Create(Context)))
        .Then(async ctx => await PointInputter.StartInput(Context, 
            ShowMouseLocationInputHelper.Create(Context),
            FixedPointInputHelper.Create(Context, ctx.Second)))
        .Then(async ctx => await PointInputter.StartInput(Context, 
            ShowMouseLocationInputHelper.Create(Context),
            FixedPointInputHelper.Create(Context, ctx.First.Second),
            FixedPointInputHelper.Create(Context, ctx.Second),
            ShowThreePointCircleOnMouseLocationInputHelper.Create(Context, ctx.First.Second, ctx.Second)))
        .Do(ctx => InputResult.SuccessTask(ctx.Flatten()));
}
