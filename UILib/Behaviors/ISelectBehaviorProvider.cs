using UILib.Shapes;

namespace UILib.Behaviors;

public interface ISelectBehaviorProvider
{
    public void ApplyBehavior<TShape>(TShape shape) where TShape : IShape;
}
