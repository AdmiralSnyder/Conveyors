using CoreLib;
using UILib.Shapes;

namespace ConveyorApp.Inputters.Helpers;

public abstract class ShowShapeInputHelper<TThis, TShape> : Inputter<TThis, Unit, CanvasInputContext>
    where TThis : ShowShapeInputHelper<TThis, TShape>, new()
    where TShape : IShape
{
    protected override void CleanupVirtual()
    {
        base.CleanupVirtual();
        Context.Canvas.RemoveFromCanvas(_TmpShape);
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
                Context.Canvas.AddToCanvas(_TmpShape);
            }
            return _TmpShape;
        }
    }
}
