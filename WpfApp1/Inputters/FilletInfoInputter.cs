using ConveyorApp.Inputters.Helpers;
using ConveyorLib.Objects;
using CoreLib;
using CoreLib.Definition;
using InputLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

internal class FilletInfoInputter : Inputter<FilletInfoInputter, ((LineDefinition LineDefinition, Point Point) LineInfo1, (LineDefinition LineDefinition, Point Point) LineInfo2), CanvasInputHelpers>
{
    protected override async Task<InputResult<((LineDefinition LineDefinition, Point Point) LineInfo1, (LineDefinition LineDefinition, Point Point) LineInfo2)>> StartAsyncVirtual()
        => await InputManager.Blank()
            .Then(async _ => await TargetLineInputter.StartInput(Context,
                Helpers.ShowUserNotes("Select the first line.")))
            .Then(async line1 => await TargetLineInputter.StartInput(Context,
                Helpers.ShowUserNotes("Select the second line."),
                Helpers.ShowPickedSelectable(line1.Second.Item1)))
            .Do(ctx => InputResult.SuccessTask(ctx.Flatten().Map(x => (x.Item1.Definition, x.Item2), x => (x.Item1.Definition, x.Item2))));
}
