using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfLib;

public class CanvasInfo : CanvasInfo<Canvas>
{
    public override TShape AddToCanvas<TShape>(TShape shape)
    {
        Canvas.Children.Add((UIElement)(object)shape);
        return shape;
    }
}
