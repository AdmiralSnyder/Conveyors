using System.Windows.Shapes;

namespace ConveyorLib;

public interface IConveyorCanvasInfo : ICanvasInfo
{
    public IConveyorShapeProvider ShapeProvider { get; set; }
}