namespace ConveyorApp.Inputters;

public class CircleInputter1 : Inputter<CircleInputter1, (Point Center, double Radius), CircleInputter1.InputStates, CanvasInputContext>
{
    public enum InputStates
    {
        SelectCenterPoint,
        SelectCircumferencePoint,
    }
    public override void Start() => InputState = InputStates.SelectCenterPoint;
}
