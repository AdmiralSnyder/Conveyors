﻿using ConveyorApp.Inputters.Helpers;
using ConveyorInputLib.Helpers;
using ConveyorLib.Objects;
using CoreLib;
using CoreLib.Definition;
using InputLib;
using InputLib.Inputters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

internal class FilletInfoInputter : Inputter<FilletInfoInputter, ((LineDefinition LineDefinition, Point Point) LineInfo1, (LineDefinition LineDefinition, Point Point) LineInfo2), WpfCanvasInputHelpers>
{
    protected override async Task<InputResult<((LineDefinition LineDefinition, Point Point) LineInfo1, (LineDefinition LineDefinition, Point Point) LineInfo2)>> StartAsyncVirtual()
        => await InputManager.BlankContext()
            .Then(async _ => await TargetLineInputter.StartInput(InputContext,
                Helpers.ShowUserNotes("Select the first line.")))
            .Then(async line1 => await TargetLineInputter.StartInput(InputContext,
                Helpers.ShowUserNotes("Select the second line."),
                Helpers.ShowPickedSelectable(line1.Last.Item1)))
            .Do(ctx => InputResult.SuccessTask(ctx.Flatten().Map(x => (x.Item1.Definition, x.Item2), x => (x.Item1.Definition, x.Item2))));
}
