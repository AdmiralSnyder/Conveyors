using ConveyorLib.Objects;
using CoreLib;
using CoreLib.Definition;
using System;
using System.Threading.Tasks;
using UILib;

namespace ConveyorApp.Inputters;

public abstract class TargetLineDefinitionInputterBase<TThis, TLineType> : Inputter<TThis, (TLineType, Point), CanvasInputContext>
    where TThis : TargetLineDefinitionInputterBase<TThis, TLineType>, new()
{
    protected override void AttachEvents()
    {
        Context.Abort += Context_Abort;
        Context.ObjectPicked += Context_ObjectPicked;
    }

    private void Context_ObjectPicked(object? sender, EventArgs<(ISelectObject, Point)> e)
        => Complete((GetLineType((ISelectObject?)e.Data.Item1), e.Data.Item2));

    protected virtual TLineType GetLineType(ISelectObject obj) => (TLineType)obj;

    private void Context_Abort(object? sender, EventArgs e) => Abort();

    protected override void DetachEvents()
    {
        Context.Abort -= Context_Abort;
        Context.ObjectPicked -= Context_ObjectPicked;
    }

    protected override void CleanupVirtual()
    {
        Context.StopObjectPickingListener();

        Context.MainWindow.InputPickManager.Disable();

        base.CleanupVirtual();
    }

    protected virtual bool ObjectCanBePicked(ISelectable obj) => obj is TLineType;

    protected override Task<InputResult<(TLineType, Point)>> StartAsyncVirtual()
    {
        Context.MainWindow.InputPickManager.Enable(ObjectCanBePicked);

        Context.StartObjectPickingListener();

        return base.StartAsyncVirtual();
    }
}

public class TargetLineDefinitionInputter : TargetLineDefinitionInputterBase<TargetLineDefinitionInputter, LineDefinition> 
{
    protected override LineDefinition GetLineType(ISelectObject obj) => ((ILineDefinition)obj).Definition;

    protected override bool ObjectCanBePicked(ISelectable obj) => obj is ILineDefinition;
}

public class TargetLineSegmentInputter : TargetLineDefinitionInputterBase<TargetLineSegmentInputter, LineSegment> { }

public class TargetLineInputter : TargetLineDefinitionInputterBase<TargetLineInputter, Line> { }
