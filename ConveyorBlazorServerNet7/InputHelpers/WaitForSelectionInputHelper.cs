using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConveyorBlazorServerNet7;
using CoreLib;
using InputLib;
using InputLib.Inputters;
using UILib.Shapes;

namespace ConveyorBlazorServerNet7.InputHelpers;

public class WaitForSelectionInputHelper : AbortingInputter<WaitForSelectionInputHelper, IEnumerable<ISelectObject>>
{
    protected override void AttachEvents()
    {
        base.AttachEvents();
        InputContext.LeftMouseButtonClicked += Context_LeftMouseButtonClicked;
    }

    protected override void DetachEvents()
    {
        base.DetachEvents();
        InputContext.LeftMouseButtonClicked -= Context_LeftMouseButtonClicked;
    }

    private void Context_LeftMouseButtonClicked(object? sender, EventArgs<Point> e)
    {
        if (AppContent.CanvasInfo.Canvas.QuadTree.Query(new(e.Data, (0, 0))) is { } shapes)
        {
            var items = shapes.Select(AppContent.CanvasInfo.ResolveShape).OfType<ITag>()
                .Select(t => t.Tag).OfType<ISelectObject>()
                .Where(so => so is not null && so.IsSelectionMatch(e.Data)).ToList();

            if (items.Any())
            {
                Complete(items);
            }
        }
    }
}
