using System.Windows.Input;
using UILib;

namespace ConveyorInputLib.Helpers;

public class ShowMouseLocationInputHelper : ShowPointInputHelper<ShowMouseLocationInputHelper>
{
    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, (Point Point, EventArgs args) e) => TmpShape.SetCenterLocation(e.Point);
}
