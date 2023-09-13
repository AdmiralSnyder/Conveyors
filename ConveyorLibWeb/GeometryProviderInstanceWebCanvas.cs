using UILib;
using UILib.Shapes;
using WebLibCanvas.Shapes;

namespace ConveyorLibWeb;

public class GeometryProviderInstanceWebCanvas : IGeometryProviderInstance
{ 
    public IPathGeometry CreatePathGeometry() => new WebCanvasPathGeometry();
}
