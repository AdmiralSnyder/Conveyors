using ConveyorLib.Shapes;
using InputLib;
using UILib.Shapes;

namespace ConveyorInputLib.Helpers;

public class ShowPathInputHelper : ShowDynamicShapeInputHelper<ShowPathInputHelper>
{
    public IEnumerable<Point> Points { get; set; }

    protected override IPath CreateShape() => ShapeProvider.CreateFreeHandLine(Points).MarkAsTemporary();

    private Point? LastPoint;
    protected override bool UpdateMousePoint(Vector point)
    {
        if (LastPoint.HasValue)
        {
            ((IPath)TmpShape).Geometry.AddLineFigure(LastPoint.Value, point);
        }
        LastPoint = point;
            
        return true;
        //if (Maths.GetCircleInfoByThreePoints((Point1, Point2, point), out var circInfo))
        //{
        //    UpdateCircle(circInfo.Center, circInfo.Radius);
        //    return true;
        //}
        //else return false;
    }

    public static ShowPathInputHelper Create(InputContextBase context, IEnumerable<Point> points)
    {
        var result = Create(context);
        result.Points = points;
        return result;
    }
}
