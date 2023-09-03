using System.Drawing;

namespace UILib.Shapes;

public interface ILine : IShape
{
    Color StrokeColor { get; set; }
    double StrokeThickness { get; set; }
    double X1 { get; set; }
    double Y1 { get; set; }
    double X2 { get; set; }
    double Y2 { get; set; }
}
