using ConveyorLib.Objects;
using CoreLib;
using System;
using System.Threading.Tasks;
using UILib;

namespace ConveyorApp.Inputters;

public abstract class SelectLineDefinitionInputterBase<TThis, TLineType> : Inputter<TThis, (TLineType, Point), CanvasInputContext>
    where TThis : SelectLineDefinitionInputterBase<TThis, TLineType>, new()
{
    protected override void AttachEvents()
    {
        Context.Abort += Context_Abort;
        Context.ObjectPicked += Context_ObjectPicked;
    }

    private void Context_ObjectPicked(object? sender, EventArgs<(ISelectObject, Point)> e)
        => Complete((GetLineType((ISelectObject?)e.Data.Item1), e.Data.Item2));
    protected virtual TLineType GetLineType(ISelectObject obj) => (TLineType)obj;

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

    protected virtual bool ObjectCanBePicked(ISelectable obj) => obj is TLineType;

    protected override Task<InputResult<(TLineType, Point)>> StartAsyncVirtual()
    {
        Context.MainWindow.PickManager.IsActive = true;
        Context.MainWindow.PickManager.ObjectFilter = ObjectCanBePicked;

        Context.StartObjectPickingListener();

        return base.StartAsyncVirtual();
    }
}

public class SelectLineDefinitionInputter : SelectLineDefinitionInputterBase<SelectLineDefinitionInputter, LineDefinition> 
{
    protected override LineDefinition GetLineType(ISelectObject obj) => ((ILineDefinition)obj).Definition;

    protected override bool ObjectCanBePicked(ISelectable obj) => obj is ILineDefinition;
}

public class SelectLineSegmentInputter : SelectLineDefinitionInputterBase<SelectLineSegmentInputter, LineSegment> { }

public class SelectLineInputter : SelectLineDefinitionInputterBase<SelectLineInputter, Line> { }
