using System.Drawing;
using System.Windows;

namespace UILib.Shapes;

public interface IStroke
{
    public Color? StrokeColor { get; set; }
    public double StrokeThickness { get; set; }
}

public interface IFill
{
    public Color? FillColor { get; set; }
}

public interface IShape : ITag
{
    bool Visible { get; set; }
    double Height { get; set; }
    double Width { get; set; }
}
