using System;
using System.Windows.Input;
using UILib.Shapes;

namespace ConveyorApp.Inputters.Helpers;

public abstract class ShowDynamicShapeInputHelper<TThis> : ShowShapeInputHelper<TThis, IShape>
    where TThis : ShowDynamicShapeInputHelper<TThis>, new()
{
    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, EventArgs e)
    {
        if (e is MouseEventArgs mea)
        {
            TmpShape.Visible = UpdateMousePoint(Context.GetPoint(mea));
        }
    }

    protected abstract bool UpdateMousePoint(Point point);
}
