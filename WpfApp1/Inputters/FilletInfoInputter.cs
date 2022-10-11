using ConveyorLib.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

internal class FilletInfoInputter : Inputter<FilletInfoInputter, ((LineDefinition LineDefinition, Point Point) LineInfo1, (LineDefinition LineDefinition, Point Point) LineInfo2), CanvasInputContext, CanvasInputHelpers>
{
    protected override async Task<InputResult<((LineDefinition LineDefinition, Point Point) LineInfo1, (LineDefinition LineDefinition, Point Point) LineInfo2)>> StartAsyncVirtual()
        => await InputManager.Blank()
            .Then(async _ => await SelectLineDefinitionInputter.StartInput(Context))
            .Then(async line1 => await SelectLineDefinitionInputter.StartInput(Context))
            .Do(ctx => InputResult.SuccessTask(ctx.Flatten()));
}
