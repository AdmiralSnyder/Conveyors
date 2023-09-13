using CoreLib;
using UILib.Shapes;

namespace UILib.Behaviors;

public static class SelectBehaviorProvider
{
    public static ISelectBehaviorProvider Instance { get; set; }
    public static TShape WithSelectBehavior<TShape>(this TShape shape)
        where TShape : IShape => shape.Modify(s => Instance?.ApplyBehavior(shape));
}
