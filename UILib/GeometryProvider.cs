using UILib.Shapes;

namespace UILib;

public interface IGeometryProviderInstance
{
    IPathGeometry CreatePathGeometry();
}


public static class GeometryProvider
{ 
    public static IGeometryProviderInstance Instance { get; set; }

    public static IPathGeometry CreatePathGeometry() => Instance.CreatePathGeometry();
}
