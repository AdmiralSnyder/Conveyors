using System;
using System.Windows.Input;
using System.Windows.Shapes;

namespace WpfLib
{
    public static class Func
    {
        public static void ApplyMouseBehaviour(Shape shape, Action<Shape> behaviour, MouseAction mouseAction = MouseAction.LeftClick) => shape.InputBindings.Add(new MouseBinding(new MyCommand<Shape>(behaviour, shape), new(MouseAction)));

    }
}
