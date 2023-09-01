using WpfLib;

namespace ConveyorLib.Wpf;


public class ConveyorCanvasInfo : CanvasInfo, IConveyorCanvasInfo
{    
    public IConveyorShapeProvider ShapeProvider { get; set; }
}
