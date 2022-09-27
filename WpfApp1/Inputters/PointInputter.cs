using CoreLib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

public class PointInputter : Inputter<PointInputter, Point, CanvasInputContext>
{
    protected override void AttachEvents()
    {
        Context.LeftMouseButtonClicked += Context_LeftMouseButtonClicked;
        Context.Abort += Context_Abort;
    }

    protected override void DetachEvents()
    {
        Context.LeftMouseButtonClicked -= Context_LeftMouseButtonClicked;
        Context.Abort -= Context_Abort;
    }

    private async void Context_LeftMouseButtonClicked(object? sender, EventArgs<Vector> e) => Complete(Context.SnapPoint(e.Data));
    private async void Context_Abort(object? sender, EventArgs e) => Abort();
}
