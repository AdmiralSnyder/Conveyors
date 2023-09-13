using UILib.Shapes;
using WpfLib.Shapes;

namespace ConveyorLib.Wpf;

public class GeometryProviderInstanceWpf : IGeometryProviderInstance
{ 
    public IPathGeometry CreatePathGeometry() => new WpfPathGeometry();
}
