using ConveyorInputLib.Helpers;
using InputLib;
using InputLib.Inputters;

namespace ConveyorInputLib.Inputters;

public class DrawInputter : AbortingInputter<DrawInputter, IEnumerable<Point>, CanvasInputHelpers>
{
    protected override async Task<InputResult<IEnumerable<Point>>> StartAsyncVirtual()
    {
        var result = await InputManager.BlankContext()
            .Then(_ => Helpers.AddData<List<Point>>())
            .Then(async ctx => await PointInputter.StartInput(InputContext,
                Helpers.ShowMouseLocation()))
            .Then(ctx => Helpers.AddToList(ctx.Previous.Last, ctx.Last))
            .Then(async ctx => await new StartDrawingInputHelper() { PointList = ctx.Previous.Last }.Run(InputContext,  // TODO NRE ctx.Previous
                Helpers.ShowFixedPoint(ctx.Last),
                Helpers.ShowPath(ctx.Previous.Last)
                ))
            .Do(ctx => InputResult.SuccessTask(ctx.Last));

        return result;
    }
}
