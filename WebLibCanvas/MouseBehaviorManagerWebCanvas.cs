using ConveyorLibWeb.Shapes;
using UILib;
using UILib.Shapes;

namespace WebLibCanvas;

public class MouseBehaviorManagerWebCanvas : IMouseBehaviorManager
{
    public void ApplyMouseBehavior(IShape shape, Action<IShape> behavior, MouseActions mouseAction = MouseActions.LeftClick)
    {
        ((WebCanvasShape)shape).BackingShape.AddMouseAction(mouseAction, behavior);
    }
}
