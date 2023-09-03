using System.Windows;

namespace UILib.Shapes;

public interface IPathGeometry
{  }

public interface IShape : ITag
{
    bool Visible { get; set; }
    double Height { get; set; }
    double Width { get; set; }
}
