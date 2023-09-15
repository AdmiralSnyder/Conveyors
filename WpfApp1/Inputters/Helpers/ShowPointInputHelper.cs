using System.Windows.Media;
using System.Windows.Shapes;
using UILib;
using UILib.Shapes;
using WpfLib;

namespace ConveyorApp.Inputters.Helpers;

public abstract class ShowPointInputHelper<TThis> : ShowShapeInputHelper<TThis, IEllipse>
    where TThis : ShowPointInputHelper<TThis>, new()
{
    protected override IEllipse CreateShape() => ShapeProvider.CreateTempPoint(default);
}
