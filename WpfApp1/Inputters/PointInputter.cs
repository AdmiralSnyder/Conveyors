using CoreLib;
using InputLib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ConveyorApp.Inputters;

public class PointInputter : Inputter<PointInputter, Point>
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

    private async void Context_LeftMouseButtonClicked(object? sender, EventArgs<Point> e) => Complete(Context.GetPoint(e.Data));
    private async void Context_Abort(object? sender, EventArgs e) => Abort();
}
