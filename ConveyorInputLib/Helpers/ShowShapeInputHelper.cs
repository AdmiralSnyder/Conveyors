using CoreLib;
using InputLib;
using UILib.Shapes;

namespace ConveyorInputLib.Helpers;

public abstract class ShowShapeInputHelper<TThis, TShape> : Inputter<TThis, Unit>
    where TThis : ShowShapeInputHelper<TThis, TShape>, new()
    where TShape : IShape
{
    protected override void CleanupVirtual()
    {
        base.CleanupVirtual();
        Context.RemoveTempShape(_TmpShape);
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
                Context.AddTempShape(_TmpShape);
            }
            return _TmpShape;
        }
    }
}
