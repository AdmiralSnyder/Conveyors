namespace UILib.Shapes;

public interface IPath : IShape, IStroke 
{
    IPathGeometry Geometry { get; set; }
}
