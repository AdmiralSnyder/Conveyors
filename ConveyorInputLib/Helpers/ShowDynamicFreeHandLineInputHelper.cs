using UILib.Shapes;
using ConveyorLib.Shapes;
using InputLib;

namespace ConveyorInputLib.Helpers;

public class ShowDynamicFreeHandLineInputHelper : ShowDynamicShapeInputHelper<ShowDynamicFreeHandLineInputHelper>
{
    protected override IShape CreateShape() => ShapeProvider.CreateFreeHandLine(Points).MarkAsTemporary();

    private List<Point> Points { get; } = [];

    internal static ShowDynamicFreeHandLineInputHelper Create(InputContextBase context, Point start)
    {
        var result = Create(context);
        result.Points.Add(start);
        return result;
    }

    protected override bool UpdateMousePoint(Vector point)
    {
        Points.Add(point);
        
        return true;
    }

}
