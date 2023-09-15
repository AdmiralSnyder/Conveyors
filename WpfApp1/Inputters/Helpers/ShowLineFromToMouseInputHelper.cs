using System.Windows.Input;
using InputLib;

namespace ConveyorApp.Inputters.Helpers;

public class ShowLineFromToMouseInputHelper : ShowLineFromToInputHelper<ShowLineFromToMouseInputHelper>
{
    public static ShowLineFromToMouseInputHelper Create(InputContextBase context, Point point1)
    {
        var result = Create(context);
        result.StartPoint = point1;
        result.EndPoint = point1;
        return result;
    }

    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    private void Context_MouseMovedInCanvas(object sender, EventArgs e)
    {
        if (e is MouseEventArgs mea)
        {
            var point = Context.GetPoint(mea);
            EndPoint = point;
        }
    }
}
