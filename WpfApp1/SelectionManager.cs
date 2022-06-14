using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class SelectionManager : IRefreshListener<IRefreshable>
    {
        public SelectionManager()
        {
            RefreshManager<IRefreshable>.RegisterRefreshListener(this);
        }

        public void Notify(IRefreshable obj)
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
                        RefreshManager<IRefreshable>.UnRegisterObserver(this, _SelectObject);
                    }
                    _SelectObject = value;
                    UpdateBoundingBox(value);
                    RefreshManager<IRefreshable>.RegisterObserver(this, value);
                }
            }
        }

        public bool HierarchicalSelection { get; set; } = true;

        public void ToggleSelectMode() => SelectMode = !SelectMode;

    }
}
