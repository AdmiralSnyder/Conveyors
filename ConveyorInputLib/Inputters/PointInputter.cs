using CoreLib;
using InputLib.Inputters;

namespace ConveyorInputLib.Inputters;

public class PointInputter : AbortingInputter<PointInputter, Point>
{
    protected override void AttachEvents()
    {
        base.AttachEvents();
        InputContext.LeftMouseButtonClicked += Context_LeftMouseButtonClicked;
    }

    protected override void DetachEvents()
    {
        base.DetachEvents();
        InputContext.LeftMouseButtonClicked -= Context_LeftMouseButtonClicked;
    }

    private async void Context_LeftMouseButtonClicked(object? sender, EventArgs<Point> e) => Complete(InputContext.GetPoint(e.Data));
}
