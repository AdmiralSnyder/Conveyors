using InputLib.Inputters;

namespace ConveyorBlazorServerNet7.InputHelpers;

public class StartDrawingInputHelper : AbortingInputter<StartDrawingInputHelper, IEnumerable<Point>>
{
    public List<Point> PointList { get; set; }

    protected override void AttachEvents()
    {
        base.AttachEvents();
        InputContext.MouseMovedInCanvas += InputContext_MouseMovedInCanvas;
    }

    protected override void DetachEvents()
    {
        base.DetachEvents();
        InputContext.MouseMovedInCanvas -= InputContext_MouseMovedInCanvas;
    }

    private void InputContext_MouseMovedInCanvas(object? sender, (Vector Point, EventArgs args) e)
    {
        PointList.Add(e.Point);
    }
}
