using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace WpfLib;

public class ShapeProvider
{
    public static Action<Shape> SelectBehaviour { get; private set; }
    public void RegisterSelectBehaviour(Action<Shape> selectBehaviour) => SelectBehaviour = selectBehaviour;

    protected Line CreateLine(TwoPoints points) => new Line()
        .SetLocation(points)
        .WithSelectBehaviour();
}

public static class ShapeProviderFunc
{
    public static T WithSelectBehaviour<T>(this T shape) where T : Shape
    {
        if (ShapeProvider.SelectBehaviour is { } sb)
        {
            shape.ApplyMouseBehaviour(sb);
        }
        return shape;
    }
}
