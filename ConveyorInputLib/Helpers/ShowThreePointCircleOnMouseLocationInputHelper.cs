using InputLib;
using UILib.Shapes;

namespace ConveyorInputLib.Helpers;

public class ShowThreePointCircleOnMouseLocationInputHelper : ShowDynamicCircleInputHelper<ShowThreePointCircleOnMouseLocationInputHelper>
{
    protected override IShape CreateShape() => ShapeProvider.CreateCircle(default, default);

    public Point Point1 { get; set; }
    public Point Point2 { get; set; }

    protected override bool UpdateMousePoint(Vector point)
    {
        if (Maths.GetCircleInfoByThreePoints((Point1, Point2, point), out var circInfo))
        {
            UpdateCircle(circInfo.Center, circInfo.Radius);
            return true;
        }
        else return false;
    }

    internal static ShowThreePointCircleOnMouseLocationInputHelper Create(InputContextBase context, Point point1, Point point2)
    {
        var result = Create(context);
        result.Point1 = point1;
        result.Point2 = point2;
        return result;
    }
}
