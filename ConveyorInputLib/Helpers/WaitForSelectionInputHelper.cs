using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib;
using InputLib;
using InputLib.Inputters;

namespace ConveyorInputLib.Helpers;

public class WaitForSelectionInputHelper : AbortingInputter<WaitForSelectionInputHelper, object>
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

    private async void Context_LeftMouseButtonClicked(object? sender, EventArgs<Point> e)
    {

        Complete(e.Data);
    }
}
