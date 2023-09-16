using UILib;
using UILib.Shapes;

namespace ConveyorInputLib.Helpers;

public abstract class ShowPointInputHelper<TThis> : ShowShapeInputHelper<TThis, IEllipse>
    where TThis : ShowPointInputHelper<TThis>, new()
{
    protected override IEllipse CreateShape() => ShapeProvider.CreateTempPoint(default);
}
