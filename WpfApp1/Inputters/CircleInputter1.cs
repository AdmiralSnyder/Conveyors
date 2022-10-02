using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

public class CircleInputter1 : Inputter<CircleInputter1, (Point Center, double Radius), CircleInputter1.InputStates, CanvasInputContext>
{
    public enum InputStates
    {
        SelectCenterPoint,
        SelectCircumferencePoint,
    }
    public override void Start()
    {
        InputState = InputStates.SelectCenterPoint;
    }



    public override async Task<InputResult<(Vector Center, double Radius)>> StartAsyncVirtual()
    {
        //var result = await InputManager.Blank()
        //    .Then(async _ => await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context)))
        //    .Then(async ctx => await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context)))
        //    .Do();

        //if (result.IsSuccess(out var points))
        //{
        //    var (startPoint, endPoint) = points.Flatten();

        //    return InputResult.Success((startPoint, (endPoint - startPoint).Length()));
        //}

        if ((await PointInputter.StartInput(Context, ShowMouseLocationInputHelper.Create(Context))).IsSuccess(out var centerPoint))
        {
            var centerPointShape = Context.AddPoint(centerPoint);
            if ((await PointInputter.StartInput(Context, ShowMouseLocationInputHelper.Create(Context))).IsSuccess(out var circPoint))
            {
                Context.RemoveShape(centerPointShape);
                return InputResult.Success((centerPoint, (circPoint - centerPoint).Length()));
            }
            else
            {
                Context.Canvas.Children.Remove(centerPointShape);
            }
        }

        return InputResult.Failure;
    }
}
