using System;
using UILib.Shapes;

namespace UILib;

public enum MouseActions : byte
{
    None = 0,
    LeftClick = 1,
    RightClick = 2,
    MiddleClick = 3,
    WheelClick = 4,
    LeftDoubleClick = 5,
    RightDoubleClick = 6,
    MiddleDoubleClick = 7
}

public interface IMouseBehaviorManager
{
    void ApplyMouseBehavior(IShape shape, Action<IShape> behavior, MouseActions mouseAction = MouseActions.LeftClick);
}


public static class MouseBehaviorManager 
{
    public static IMouseBehaviorManager Instance { get; set; }

    public static TShape WithMouseBehavior<TShape>(this TShape shape, Action<IShape> action, MouseActions mouseAction = MouseActions.LeftClick)
        where TShape : IShape
    {
        Instance?.ApplyMouseBehavior(shape, action, mouseAction);
        return shape;
    }

}
