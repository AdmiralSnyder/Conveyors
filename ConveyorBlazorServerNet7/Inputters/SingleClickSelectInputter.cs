using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConveyorBlazorServerNet7.InputHelpers;
using InputLib;
using InputLib.Inputters;

namespace ConveyorBlazorServerNet7.Inputters;

public class SingleClickSelectInputter : AbortingInputter<SingleClickSelectInputter, IEnumerable<ISelectObject>>
{
    protected override async Task<InputResult<IEnumerable<ISelectObject>>> StartAsyncVirtual()
    {
        var result = await InputManager.Blank()
            .Then(async _ => await WaitForSelectionInputHelper.StartInput(Context))
            .Do(ctx => InputResult.SuccessTask(ctx.Second));

        return result;
    }
}
