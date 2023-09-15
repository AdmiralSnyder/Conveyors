﻿using System;
using System.Windows.Input;
using InputLib;

namespace ConveyorApp.Inputters.Helpers;

public class ShowCalculatedPointInputHelper : ShowPointInputHelper<ShowCalculatedPointInputHelper>
{
    private Func<Vector, Vector> CalculationOnMouse;

    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    public static ShowCalculatedPointInputHelper Create(InputContextBase context, Func<Point, Point> calculationOnMouse)
    {
        var result = Create(context);
        result.CalculationOnMouse = calculationOnMouse;
        return result;
    }

    private void Context_MouseMovedInCanvas(object sender, EventArgs e)
    {
        if (e is MouseEventArgs mea)
        {
            var mousePoint = Context.GetPoint(mea);
            var point = CalculationOnMouse(mousePoint);
            TmpShape.SetCenterLocation(point);
        }
    }
}