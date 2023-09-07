using WpfLib;

namespace ConveyorLib.Wpf;

public class ConveyorCanvasInfo : WpfCanvasInfo, IConveyorCanvasInfo
{
    IConveyorShapeProvider IConveyorCanvasInfo.ShapeProvider 
    {
        get => (IConveyorShapeProvider)ShapeProvider;
        set => ShapeProvider = value;
    }
}
