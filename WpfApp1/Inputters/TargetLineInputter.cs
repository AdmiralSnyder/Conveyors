using ConveyorLib.Objects;
using CoreLib;
using CoreLib.Definition;
using InputLib;
using InputLib.Inputters;
using System;
using System.Threading.Tasks;
using UILib;

namespace ConveyorApp.Inputters;

public abstract class TargetLineDefinitionInputterBase<TThis, TLineType> : AbortingInputter<TThis, (TLineType, Point)>
    where TThis : TargetLineDefinitionInputterBase<TThis, TLineType>, new()
{
    protected override void AttachEvents()
    {
        base.AttachEvents();
        InputContext.ObjectPicked += Context_ObjectPicked;
    }

    private void Context_ObjectPicked(object? sender, EventArgs<(ISelectObject, Point)> e)
        => Complete((GetLineType((ISelectObject?)e.Data.Item1), e.Data.Item2));

    protected virtual TLineType GetLineType(ISelectObject obj) => (TLineType)obj;


    protected override void DetachEvents()
    {
        base.DetachEvents();
        InputContext.ObjectPicked -= Context_ObjectPicked;
    }

    protected override void CleanupVirtual()
    {
        ((WpfCanvasInputContext)InputContext).StopObjectPickingListener();
        ((WpfCanvasInputContext)InputContext).ViewModel.InputPickManager.Disable();

        base.CleanupVirtual();
    }

    protected virtual bool ObjectCanBePicked(ISelectable obj) => obj is TLineType;

    protected override Task<InputResult<(TLineType, Point)>> StartAsyncVirtual()
    {
        ((WpfCanvasInputContext)InputContext).ViewModel.InputPickManager.Enable(ObjectCanBePicked);
        ((WpfCanvasInputContext)InputContext).StartObjectPickingListener();

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
