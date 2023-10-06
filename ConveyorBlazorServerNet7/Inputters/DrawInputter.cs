using ConveyorBlazorServerNet7.InputHelpers;
using InputLib;
using InputLib.Inputters;

namespace ConveyorBlazorServerNet7.Inputters;

public class DrawInputter : AbortingInputter<DrawInputter, object>
{
    protected override async Task<InputResult<object>> StartAsyncVirtual()
    {

        var result = await InputManager.Blank()
            .Then(async _ => await StartDrawingInputHelper.StartInput(Context))
            .Do(ctx => InputResult.SuccessTask(ctx.Second));

        return result;
    }
}
