﻿using CoreLib;
using InputLib.Inputters;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ConveyorInputLib.Inputters;

public class PointInputter : AbortingInputter<PointInputter, Point>
{
    protected override void AttachEvents()
    {
        base.AttachEvents();
        Context.LeftMouseButtonClicked += Context_LeftMouseButtonClicked;
    }

    protected override void DetachEvents()
    {
        base.DetachEvents();
        Context.LeftMouseButtonClicked -= Context_LeftMouseButtonClicked;
    }

    private async void Context_LeftMouseButtonClicked(object? sender, EventArgs<Point> e) => Complete(Context.GetPoint(e.Data));
}