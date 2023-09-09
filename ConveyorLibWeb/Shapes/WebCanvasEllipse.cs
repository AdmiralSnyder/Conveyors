using System.Drawing;
using UILib.Shapes;

namespace ConveyorLibWeb.Shapes;

public class WebCanvasEllipse: WebCanvasShape<WebEllipse>, IEllipse
{
    public WebCanvasEllipse(): base(new()) { }

    public Color? FillColor { get; set; }
    public Color? StrokeColor { get; set; }
    public double StrokeThickness { get; set; }
}
