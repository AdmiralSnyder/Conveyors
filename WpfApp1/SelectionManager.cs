using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public class SelectionManager : IRefreshListener<ISelectable>, INotifyPropertyChanged
    {
        public SelectionManager()
        {
            RefreshManager<ISelectable>.RegisterRefreshListener(this);
        }

        public void Notify(ISelectable obj)
        {
            if (obj is ISelectObject notifyObj && notifyObj == SelectedObject)
            {
                UpdateBoundingBox(notifyObj);
            }
        }

        public bool SelectMode { get; set; }

        public Action<ISelectObject> UpdateBoundingBox { get; set; }

        private ISelectObject _SelectObject;
        public ISelectObject SelectedObject
        {
            get => _SelectObject;
            set
            {
                if (_SelectObject != value)
                {
                    if (_SelectObject != null)
                    {
                        RefreshManager<ISelectable>.UnRegisterObserver(this, _SelectObject);
                    }
                    _SelectObject = value;
                    UpdateBoundingBox(value);
                    RefreshManager<ISelectable>.RegisterObserver(this, value);
                    OnPropertyChanged(nameof(SelectedObject));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new(name));
        }

        public bool HierarchicalSelection { get; set; } = true;

        public void ToggleSelectMode() => SelectMode = !SelectMode;

    }
}
