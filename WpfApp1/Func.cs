using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Func
    {
        public static void Setter<T>(ref T backingField, T value, Action<T> onChangeAction)
        {
            if (!object.Equals(backingField, value))
            {
                backingField = value;
                onChangeAction(value);
            }
        }

        public static void Setter<T>(ref T backingField, T value, Action onChangeAction)
        {
            if (!object.Equals(backingField, value))
            {
                backingField = value;
                onChangeAction();
            }
        }
    }
}
