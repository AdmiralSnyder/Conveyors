namespace ConveyorApp.Inputters.Helpers;

public class ShowCircleByRadiusInputHelper : ShowDynamicCircleInputHelper<ShowCircleByRadiusInputHelper>
{
    public Point Center { get; set; }

    internal static ShowCircleByRadiusInputHelper Create(CanvasInputContext context, Point point1)
    {
        var result = Create(context);
        result.Center = point1;
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
