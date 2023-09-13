namespace ConveyorApp.Inputters.Helpers;

public class ShowLineFromToFixedInputHelper : ShowLineFromToInputHelper<ShowLineFromToFixedInputHelper>
{
    public static ShowLineFromToFixedInputHelper Create(CanvasInputContext context, Point point1, Point point2)
    {
        var result = Create(context);
        result.StartPoint = point1;
        result.EndPoint = point2;
        return result;
    }
}
