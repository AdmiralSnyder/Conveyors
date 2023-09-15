using UILib.Shapes;
using WpfLib.Shapes;

namespace WpfLib;

public class MouseBehaviorManagerWpf : IMouseBehaviorManager
{
    public void ApplyMouseBehavior(IShape shape, Action<IShape> behavior, MouseActions mouseAction = MouseActions.LeftClick)
    => ((WpfShape)shape).BackingShape.InputBindings.Add(new MouseBinding(new UICommand<IShape>(behavior, shape), new((MouseAction)mouseAction)));
}
