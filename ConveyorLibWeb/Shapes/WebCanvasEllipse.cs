using UILib.Shapes;

namespace ConveyorLibWeb.Shapes;

public class WebCanvasEllipse: WebCanvasShape<WebEllipse>, IEllipse
{
    public WebCanvasEllipse(WebEllipse ellipse): base(ellipse) { }
}
