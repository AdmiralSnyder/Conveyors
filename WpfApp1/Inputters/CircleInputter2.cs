using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

class CircleInputter2 : Inputter<CircleInputter2, (Point Point1, Point Point2, Point Point3), CanvasInputContext>
{
    public override async Task<InputResult<(Point Point1, Point Point2, Point Point3)>> StartAsyncVirtual()
    {
        var result = await InputManager.Blank()
            .Then(async _ => await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context)))
            .Then(async ctx => await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context)))
            .Then(async ctx => await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context)))
            .Do();

        if (result.IsSuccess(out var points) && points.Flatten2(out var allPoints))
        {
            return InputResult.Success((allPoints.Item1, allPoints.Item2, allPoints.Item1));
        }

        //if ((await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context))).IsSuccess(out var point1))
        //{
        //    var point1Shape = Context.AddPoint(point1);
        //    if ((await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context))).IsSuccess(out var point2))
        //    {
        //        var point2Shape = Context.AddPoint(point2);
        //        if ((await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context))).IsSuccess(out var point3))
        //        {
        //            Context.RemoveShape(point1Shape);
        //            Context.RemoveShape(point2Shape);

        //            return InputResult.Success((point1, point2, point3));
        //        }
        //    }
        //}

        return InputResult.Failure;
    }
}
