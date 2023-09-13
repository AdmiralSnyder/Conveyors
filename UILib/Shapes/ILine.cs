using System.Drawing;

namespace UILib.Shapes;

public interface ILine : IShape, IStroke
{
    double X1 { get; set; }
    double Y1 { get; set; }
    double X2 { get; set; }
    double Y2 { get; set; }
}
