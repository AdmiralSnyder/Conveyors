using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    internal class CursorChanger : PropertyChanger<Cursor>
    {
        public CursorChanger(FrameworkElement element, Cursor newCursor)
            : base(() => element.Cursor, c => element.Cursor = c, newCursor)
        { }
    }

    internal class PropertyChanger<T> : StateChanger
    {
        public PropertyChanger(Func<T> getOldValueFunc, Action<T> setNewValueAction, T newValue)
        {
            OldValue = getOldValueFunc();
            SetNewValueAction = setNewValueAction;
            SetNewValueAction(newValue);
        }

        private readonly Action<T> SetNewValueAction;
        private readonly T OldValue;

        protected override void Reverse() => SetNewValueAction(OldValue);
    }

    internal abstract class StateChanger : IDisposable
    {
        protected abstract void Reverse();

        public void Dispose() => Reverse();
    }
}
