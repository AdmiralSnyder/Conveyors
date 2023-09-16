using System;
using System.Windows.Input;
using UILib.Shapes;

namespace ConveyorInputLib.Helpers;

public abstract class ShowDynamicShapeInputHelper<TThis> : ShowShapeInputHelper<TThis, IShape>
    where TThis : ShowDynamicShapeInputHelper<TThis>, new()
{
    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, (Point Point, EventArgs args) e) => TmpShape.Visible = UpdateMousePoint(e.Point);

    protected abstract bool UpdateMousePoint(Point point);
}
