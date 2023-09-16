using UILib.Shapes;
using ConveyorLib.Shapes;
using UILib;

namespace ConveyorInputLib.Helpers;

public abstract class ShowDynamicCircleInputHelper<TThis> : ShowDynamicShapeInputHelper<TThis>
    where TThis : ShowDynamicCircleInputHelper<TThis>, new()
{
    protected override IShape CreateShape() => ShapeProvider.CreateCircle(default, default).MarkAsTemporary();
    protected void UpdateCircle(Point center, double radius)
    {
        TmpShape.SetCenterLocation(center);
        TmpShape.Height = 2 * radius + 1;
        TmpShape.Width = 2 * radius + 1;
    }
}
