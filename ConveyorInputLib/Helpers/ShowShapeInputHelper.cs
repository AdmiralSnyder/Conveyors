using CoreLib;
using InputLib.Inputters;
using UILib.Shapes;

namespace ConveyorInputLib.Helpers;

public abstract class ShowShapeInputHelper<TThis, TShape> : Inputter<TThis, Unit>
    where TThis : ShowShapeInputHelper<TThis, TShape>, new()
    where TShape : IShape
{
    protected override void CleanupVirtual()
    {
        base.CleanupVirtual();
        InputContext.RemoveTempShape(_TmpShape);
    }

    private TShape? _TmpShape;

    protected abstract TShape CreateShape();

    protected TShape TmpShape
    {
        get
        {
            if (_TmpShape is null)
            {
                _TmpShape = CreateShape();
                InputContext.AddTempShape(_TmpShape);
            }
            return _TmpShape;
        }
    }
}
