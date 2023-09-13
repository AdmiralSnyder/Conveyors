using System.Windows.Input;

namespace ConveyorApp.Inputters.Helpers;

public class ShowMouseLocationInputHelper : ShowPointInputHelper<ShowMouseLocationInputHelper>
{
    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, MouseEventArgs e)
    {
        var point = Context.GetSnappedCanvasPoint(e);
        TmpShape.SetCenterLocation(point);
    }
}
