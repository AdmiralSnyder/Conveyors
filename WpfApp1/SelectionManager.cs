using CoreLib;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfLib;

namespace ConveyorApp;

public abstract class ChooseObjectManager : INotifyPropertyChangedImpl, IRefreshListener<ISelectable>
{
    public bool IsActive 
    {
        get => _IsActive;
        protected set => Func.Setter(ref _IsActive, value, isActive =>
        {
            if (!isActive && ClearOnInactive)
            {
                ChosenObject = null;
            }
        });
    }

    protected bool ClearOnInactive { get; set; }

    public void Notify(ISelectable obj)
    {
        if (obj is ISelectObject notifyObj && notifyObj == ChosenObject)
        {
            UpdateBoundingBox(notifyObj);
        }
    }

    private ISelectObject? _ChosenObject;
    private bool _IsActive;

    public ISelectObject? ChosenObject
    {
        get => _ChosenObject;
        set => this.SetterInpc(ref _ChosenObject, value, (oldValue, newValue) =>
        {
            RefreshManager<ISelectable>.UnRegisterObserver(this, oldValue);

            UpdateBoundingBox(value);
            RefreshManager<ISelectable>.RegisterObserver(this, value);
            ChosenObjectChanged?.Invoke(this, new((ChosenObject, MousePosition.AsPoint())));
        });
    }

    public event EventHandler<EventArgs<(ISelectable?, Point)>>? ChosenObjectChanged;

    public abstract void UpdateBoundingBox(ISelectObject? selectObject);

    public System.Windows.Point MousePosition { get; internal set; }

    public ObjectHighlighter? Highlighter { get; protected set; }
}

public class InputPickManager : ChooseObjectManager
{
    public InputPickManager() => ClearOnInactive = true;

    private Func<ISelectable, bool>? ObjectFilter { get; set; }

    public void Enable(Func<ISelectable, bool> objectFilter)
    {
        IsActive = true;
        ObjectFilter = objectFilter;
    }

    public void Disable()
    {
        ObjectFilter = null;
        IsActive = false;
    }

    public bool QueryCanPickObject(ISelectable selectable) => ObjectFilter?.Invoke(selectable) ?? false;

    public override void UpdateBoundingBox(ISelectObject? selectObject) { }
}

public abstract class TargetObjectManager : ChooseObjectManager
{
    private Func<ISelectable, bool>? ObjectFilter { get; set; }

    public void Enable(Func<ISelectable, bool> objectFilter)
    {
        IsActive = true;
        ObjectFilter = objectFilter;
    }

    public void Disable()
    {
        ObjectFilter = null;
        IsActive = false;
    }

    public bool QueryCanPickObject(ISelectable selectable) => ObjectFilter?.Invoke(selectable) ?? false;
}

public abstract class SelectionManager : ChooseObjectManager
{
    public SelectionManager() => RefreshManager<ISelectable>.RegisterRefreshListener(this);

    public bool HierarchicalSelection { get; set; } = true;

    public void ToggleSelectMode() => IsActive = !IsActive;
}
