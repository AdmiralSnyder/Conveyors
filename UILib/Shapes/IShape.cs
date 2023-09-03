using System.Windows;

namespace UILib.Shapes;

public interface IShape : ITag
{
    Visibility Visibility { get; set; }
    double Height { get; set; }
    double Width { get; set; }
}
