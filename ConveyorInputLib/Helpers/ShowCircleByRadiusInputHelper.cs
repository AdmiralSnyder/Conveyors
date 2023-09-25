using InputLib;

namespace ConveyorInputLib.Helpers;

public class ShowCircleByRadiusInputHelper : ShowDynamicCircleInputHelper<ShowCircleByRadiusInputHelper>
{
    public Point Center { get; set; }

    internal static ShowCircleByRadiusInputHelper Create(InputContextBase context, Point center)
    {
        var result = Create(context);
        result.Center = center;
        return result;
    }

    protected override bool UpdateMousePoint(Vector point)
    {
        if (point != Center)
        {
            UpdateCircle(Center, (Center - point).Length());
            return true;
        }
        else return false;
    }
}
