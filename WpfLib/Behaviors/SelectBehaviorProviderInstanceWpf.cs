using System;
using UILib.Behaviors;
using UILib.Shapes;

namespace WpfLib.Behaviors;

public class SelectBehaviorProviderInstanceWpf : ISelectBehaviorProvider
{
    public static Action<IShape>? SelectBehaviour { get; private set; }
    public static void RegisterSelectBehavior(Action<IShape> selectBehaviour) => SelectBehaviour = selectBehaviour;

    public void ApplyBehavior<TShape>(TShape shape) where TShape : IShape
    {
        if (SelectBehaviour is { } sb)
        {
            shape.WithMouseBehavior(sb);
        }
    }
}
