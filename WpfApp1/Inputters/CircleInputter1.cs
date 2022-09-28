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
        if ((await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context))).IsSuccess(out var centerPoint))
        {
            var centerPointShape = Context.AddPoint(centerPoint);
            if ((await PointInputter.StartInput(Context, ShowPointerLocationInputHelper.Create(Context))).IsSuccess(out var circPoint))
            {
                Context.RemoveShape(centerPointShape);
                return InputResult.Success((centerPoint, (circPoint - centerPoint).Length()));
            }
        }

        return InputResult.Failure;
    }
}
