using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

class CircleInputter2 : Inputter<CircleInputter2, (Point Point1, Point Point2, Point Point3), CanvasInputContext>
{
    public override async Task<InputResult<(Point Point1, Point Point2, Point Point3)>> StartAsyncVirtual()
    {
        if ((await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context))).IsSuccess(out var point1))
        {
            var point1Shape = Context.AddPoint(point1);
            if ((await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context))).IsSuccess(out var point2))
            {
                var point2Shape = Context.AddPoint(point2);
                if ((await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context))).IsSuccess(out var point3))
                {
                    Context.RemoveShape(point1Shape);
                    Context.RemoveShape(point2Shape);

                    return InputResult.Success((point1, point2, point3));
                }
            }
        }

        return InputResult.Failure;
    }
}
