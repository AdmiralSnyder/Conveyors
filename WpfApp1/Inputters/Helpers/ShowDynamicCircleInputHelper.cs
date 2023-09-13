using UILib.Shapes;
using ConveyorLib.Shapes;

namespace ConveyorApp.Inputters.Helpers;

public abstract class ShowDynamicCircleInputHelper<TThis> : ShowDynamicShapeInputHelper<TThis>
    where TThis : ShowDynamicCircleInputHelper<TThis>, new()
{
    protected override IShape CreateShape() => Context.ViewModel.ShapeProvider.CreateCircle(default, default).MarkAsTemporary();
    protected void UpdateCircle(Point center, double radius)
    {
        TmpShape.SetCenterLocation(center);
        TmpShape.Height = 2 * radius + 1;
        TmpShape.Width = 2 * radius + 1;
    }
}
