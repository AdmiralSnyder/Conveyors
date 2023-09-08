using WpfLib;

namespace ConveyorLib.Wpf;

public class WpfConveyorCanvasInfo : WpfCanvasInfo, IConveyorCanvasInfo
{
    IConveyorShapeProvider IConveyorCanvasInfo.ShapeProvider 
    {
        get => (IConveyorShapeProvider)ShapeProvider;
        set => ShapeProvider = value;
    }
}
