namespace ConveyorApp.Inputters.Helpers;

public class ShowCircleByDiameterInputHelper : ShowDynamicCircleInputHelper<ShowCircleByDiameterInputHelper>
{
    public Point Point1 { get; set; }

    internal static ShowCircleByDiameterInputHelper Create(CanvasInputContext context, Point point1)
    {
        var result = Create(context);
        result.Point1 = point1;
        return result;
    }

    protected override bool UpdateMousePoint(Vector point)
    {
        if (Maths.GetCircleInfoByDiameter((Point1, point), out var info, out _))
        {
            UpdateCircle(info.Center, info.Radius);
            return true;
        }
        else return false;
    }
}
