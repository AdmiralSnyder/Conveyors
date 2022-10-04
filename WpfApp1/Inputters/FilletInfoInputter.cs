using ConveyorLib.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

internal class FilletInfoInputter : Inputter<FilletInfoInputter, (Line, Line), CanvasInputContext, CanvasInputHelpers>
{
    protected override async Task<InputResult<(Line, Line)>> StartAsyncVirtual()
        => await InputManager.Blank()
            .Then(async _ => await SelectLineInputter.StartInput(Context))
            .Then(async line1 => await SelectLineInputter.StartInput(Context))
            .Do(ctx => InputResult.SuccessTask(ctx.Flatten()));

    //    if ((await ).IsSuccess(out var line1))
    //    {
    //        if ((await SelectLineInputter.Create(InputContext).StartAsync()).IsSuccess(out var line2))
    //        {
    //            var angle = Maths.AngleBetween(line1.Vector, line2.Vector);

    //            if (Maths.GetCrossingPoint((line1.ReferencePoint1, line1.ReferencePoint2), (line2.ReferencePoint1, line2.ReferencePoint2), out var crossingPoint))
    //            {
    //                AddPoint(crossingPoint);

    //                var radius = 25d;
    //                var tangent = Math.Tan(angle.CounterAngle().Radians / 2);

    //                var a = tangent * radius;
    //                var unitVector1 = line1.Vector.Normalize();
    //                var start1 = crossingPoint + unitVector1.Multiply(a);

    //                var unitVector2 = line2.Vector.Normalize();
    //                var start2 = crossingPoint + unitVector2.Multiply(a);

    //                bool largeArc = false;
    //                SweepDirection swDir = SweepDirection.Clockwise;

    //                var pg = new PathGeometry();

    //                pg.Figures.Add(new()
    //                {
    //                    StartPoint = start1,
    //                    Segments = { new ArcSegment(start2, new(radius, radius), 0, largeArc, swDir, true) }
    //                });

    //                var shape = ShapeProvider.CreateCircleSectorArc(pg, true);
    //                TheCanvas.Children.Add(shape);
    //            }
    //        }
    //    }
    //}
}
