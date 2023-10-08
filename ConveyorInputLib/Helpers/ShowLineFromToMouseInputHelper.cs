using System.Windows.Input;
using InputLib;

namespace ConveyorInputLib.Helpers;

public class ShowLineFromToMouseInputHelper : ShowLineFromToInputHelper<ShowLineFromToMouseInputHelper>
{
    public static ShowLineFromToMouseInputHelper Create(InputContextBase context, Point point1)
    {
        var result = Create(context);
        result.StartPoint = point1;
        result.EndPoint = point1;
        return result;
    }

    protected override void AttachEvents() => InputContext.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => InputContext.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, (Point Point, EventArgs args) e) => EndPoint = e.Point;
}
