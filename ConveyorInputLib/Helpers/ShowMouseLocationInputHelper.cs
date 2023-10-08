using System.Windows.Input;
using UILib;

namespace ConveyorInputLib.Helpers;

public class ShowMouseLocationInputHelper : ShowPointInputHelper<ShowMouseLocationInputHelper>
{
    protected override void AttachEvents() => InputContext.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => InputContext.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, (Point Point, EventArgs args) e) => TmpShape.SetCenterLocation(InputContext.GetPoint(e.Point));
}
