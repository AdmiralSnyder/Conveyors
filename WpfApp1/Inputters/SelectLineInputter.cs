using ConveyorLib.Objects;
using CoreLib;
using System;
using System.Threading.Tasks;
using UILib;

namespace ConveyorApp.Inputters;

public class SelectLineInputter : Inputter<SelectLineInputter, Line, CanvasInputContext>
{
    protected override void AttachEvents()
    {
        Context.Abort += Context_Abort;
        Context.ObjectPicked += Context_ObjectPicked;
    }

    private void Context_ObjectPicked(object? sender, EventArgs<ISelectObject> e)
    {
        Complete((Line)e.Data);
    }

    private void Context_Abort(object? sender, EventArgs e) { }

    protected override void DetachEvents()
    {
        Context.Abort -= Context_Abort;
        Context.ObjectPicked -= Context_ObjectPicked;
    }

    protected override void CleanupVirtual()
    {
        Context.StopObjectPickingListener();

        Context.MainWindow.PickManager.IsActive = false;
        Context.MainWindow.PickManager.ObjectFilter = null;

        base.CleanupVirtual();
    }

    protected override Task<InputResult<Line>> StartAsyncVirtual()
    {
        Context.MainWindow.PickManager.IsActive = true;
        Context.MainWindow.PickManager.ObjectFilter = x => x is Line;
        
        Context.StartObjectPickingListener();
        
        return base.StartAsyncVirtual();
    }
}
