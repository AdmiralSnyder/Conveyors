using ConveyorLib.Shapes;
using InputLib;
using UILib.Shapes;

namespace ConveyorInputLib.Helpers;

public class ShowPathInputHelper : ShowShapeInputHelper<ShowPathInputHelper, IPath>
{
    public IEnumerable<Point> Points { get; set; }

    protected override IPath CreateShape() => ShapeProvider.CreateFreeHandLine(Points).MarkAsTemporary();

    public static ShowPathInputHelper Create(InputContextBase context, IEnumerable<Point> points)
    {
        var result = Create(context);
        result.Points = points;
        return result;
    }
}
