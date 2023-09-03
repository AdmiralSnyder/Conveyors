using System.Windows.Media;

namespace UILib.Shapes;

public interface ILine : IShape
{
    Brush Stroke { get; set; }
    double StrokeThickness { get; set; }
    double X1 { get; set; }
    double Y1 { get; set; }
    double X2 { get; set; }
    double Y2 { get; set; }
}
