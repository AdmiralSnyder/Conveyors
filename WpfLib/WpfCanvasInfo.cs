using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UILib.Shapes;
using WpfLib.Shapes;

namespace WpfLib;

public class WpfCanvasInfo : CanvasInfo<Canvas>
{
    public override TShape AddToCanvas<TShape>(TShape shape)
    {
        Canvas.Children.Add(((WpfShape)(object)shape).BackingShape);
        return shape;
    }

    public override void BeginInvoke<T>(IShape shape, Action<T> action, T value) => ((WpfShape)(object)shape).BackingShape.Dispatcher.BeginInvoke(action, value);

    public override TShape RemoveFromCanvas<TShape>(TShape shape)
    {
        Canvas.Children.Remove(((WpfShape)(object)shape).BackingShape);
        return shape;
    }
}
