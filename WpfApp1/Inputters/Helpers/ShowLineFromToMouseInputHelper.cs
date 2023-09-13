using System.Windows.Input;

namespace ConveyorApp.Inputters.Helpers;

public class ShowLineFromToMouseInputHelper : ShowLineFromToInputHelper<ShowLineFromToMouseInputHelper>
{
    public static ShowLineFromToMouseInputHelper Create(CanvasInputContext context, Point point1)
    {
        var result = Create(context);
        result.StartPoint = point1;
        result.EndPoint = point1;
        return result;
    }

    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, MouseEventArgs e)
    {
        var point = Context.GetSnappedCanvasPoint(e);
        EndPoint = point;
    }
}
