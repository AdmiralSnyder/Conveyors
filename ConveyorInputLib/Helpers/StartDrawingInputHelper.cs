using CoreLib;
using InputLib.Inputters;

namespace ConveyorInputLib.Helpers;

public class StartDrawingInputHelper : AbortingInputter<StartDrawingInputHelper, IEnumerable<Point>>
{
    public List<Point> PointList { get; set; }

    protected override void AttachEvents()
    {
        base.AttachEvents();
        InputContext.MouseMovedInCanvas += InputContext_MouseMovedInCanvas;
        InputContext.LeftMouseButtonClicked += InputContext_LeftMouseButtonClicked;
    }

    private void InputContext_LeftMouseButtonClicked(object? sender, EventArgs<Point> e)
    {
        Complete(PointList);
    }

    protected override void DetachEvents()
    {
        base.DetachEvents();
        InputContext.MouseMovedInCanvas -= InputContext_MouseMovedInCanvas;
        InputContext.LeftMouseButtonClicked -= InputContext_LeftMouseButtonClicked;

    }

    private void InputContext_MouseMovedInCanvas(object? sender, (Point Point, EventArgs args) e)
    {
        PointList.Add(e.Point);
    }
}
