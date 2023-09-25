using ConveyorLib.Shapes;
using UILib.Shapes;

namespace ConveyorLib;

public interface IConveyorCanvasInfo : ICanvasInfo
{
    IConveyorShapeProvider ShapeProvider { get; set; }
}