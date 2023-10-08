using System;
using System.Windows.Input;
using InputLib;
using UILib;

namespace ConveyorInputLib.Helpers;

public class ShowCalculatedPointInputHelper : ShowPointInputHelper<ShowCalculatedPointInputHelper>
{
    private Func<Vector, Vector> CalculationOnMouse;

    protected override void AttachEvents() => InputContext.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => InputContext.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    public static ShowCalculatedPointInputHelper Create(InputContextBase context, Func<Point, Point> calculationOnMouse)
    {
        var result = Create(context);
        result.CalculationOnMouse = calculationOnMouse;
        return result;
    }

    private void Context_MouseMovedInCanvas(object sender, (Point Point, EventArgs args) e)
    {
        var point = CalculationOnMouse(e.Point);
        TmpShape.SetCenterLocation(point);
    }
}