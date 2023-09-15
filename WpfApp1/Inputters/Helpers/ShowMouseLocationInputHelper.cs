using System.Windows.Input;

namespace ConveyorApp.Inputters.Helpers;

public class ShowMouseLocationInputHelper : ShowPointInputHelper<ShowMouseLocationInputHelper>
{
    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, EventArgs e)
    {
        if (e is MouseEventArgs mea)
        {
            var point = Context.GetPoint(mea);
            TmpShape.SetCenterLocation(point);
        }
    }
}
