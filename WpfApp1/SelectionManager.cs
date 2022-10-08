using CoreLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UILib;

namespace ConveyorApp;

public abstract class ChooseManager : IRefreshListener<ISelectable>, INotifyPropertyChanged
{
    public bool IsActive { get; set; }

    public void Notify(ISelectable obj)
    {
        if (obj is ISelectObject notifyObj && notifyObj == ChosenObject)
        {
            UpdateBoundingBox(notifyObj);
        }
    }

    private ISelectObject _ChosenObject;
    public ISelectObject ChosenObject
    {
        get => _ChosenObject;
        set
        {
            if (_ChosenObject != value)
            {
                if (_ChosenObject != null)
                {
                    RefreshManager<ISelectable>.UnRegisterObserver(this, _ChosenObject);
                }
                _ChosenObject = value;
                UpdateBoundingBox(value);
                RefreshManager<ISelectable>.RegisterObserver(this, value);
                OnPropertyChanged(nameof(ChosenObject));

                ChosenObjectChanged?.Invoke(this, new((ChosenObject, MousePosition)));
            }
        }
    }

    public event EventHandler<EventArgs<(ISelectable, Point)>> ChosenObjectChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new(name));

    public Action<ISelectObject> UpdateBoundingBox { get; set; }
    public System.Windows.Point MousePosition { get; internal set; }
}

public class PickManager : ChooseManager
{
    public Func<ISelectable, bool>? ObjectFilter { get; internal set; }

    public bool QueryCanPickObject(ISelectable selectable) => ObjectFilter(selectable);
}

public class SelectionManager : ChooseManager
{
    public SelectionManager() => RefreshManager<ISelectable>.RegisterRefreshListener(this);

    public bool HierarchicalSelection { get; set; } = true;

    public void ToggleSelectMode() => IsActive = !IsActive;

}
