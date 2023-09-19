using ConveyorLib;
using ConveyorLib.Shapes;
using ConveyorLibWeb.Shapes;
using UILib.Shapes;

namespace ConveyorBlazorServerNet7;

public class WebCanvasInfo : CanvasInfo<WebCanvas>, IConveyorCanvasInfo
{
    public override TShape AddToCanvas<TShape>(TShape shape)
    {
        Canvas.Children.Add(((WebCanvasShape)(object)shape).BackingShape);

        return shape;
    }

    public override void BeginInvoke<T>(IShape shape, Action<T> action, T value)
    {
        throw new NotImplementedException();
    }

    IConveyorShapeProvider IConveyorCanvasInfo.ShapeProvider
    {
        get => (IConveyorShapeProvider)ShapeProvider;
        set => ShapeProvider = value;
    }

    public override TShape RemoveFromCanvas<TShape>(TShape shape)
    {
        Canvas.Children.Remove(((WebCanvasShape)(object)shape).BackingShape);
        return shape;
    }
}
