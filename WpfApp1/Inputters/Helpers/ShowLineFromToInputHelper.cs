using CoreLib;
using UILib.Shapes;

namespace ConveyorApp.Inputters.Helpers;

public abstract class ShowLineFromToInputHelper<TThis> : ShowShapeInputHelper<TThis, ILine>
where TThis : ShowLineFromToInputHelper<TThis>, new()
{
    private Point _StartPoint;

    protected Point StartPoint
    {
        get => _StartPoint;
        set => Func.Setter(ref _StartPoint, value, () =>
        {
            TmpShape.X1 = StartPoint.X;
            TmpShape.Y1 = StartPoint.Y;
        });
    }

    private Point _EndPoint;

    protected Point EndPoint
    {
        get => _EndPoint;
        set => Func.Setter(ref _EndPoint, value, () =>
        {
            TmpShape.X2 = EndPoint.X;
            TmpShape.Y2 = EndPoint.Y;
        });
    }

    protected override ILine CreateShape() => ShapeProvider.CreateTempLine(default);
}
