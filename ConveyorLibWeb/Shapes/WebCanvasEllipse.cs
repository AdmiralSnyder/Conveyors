using UILib.Shapes;

namespace ConveyorLibWeb.Shapes;

public class WebCanvasEllipse: WebCanvasShape<WebEllipse>, IEllipse
{
    public WebCanvasEllipse(WebEllipse ellipse): base(ellipse) { }
    public Point Center
    {
        get => BackingObject.Center;
        set => BackingObject.Center = value;
    }

    public double Radius
    {
        get => BackingObject.Radius;
        set => BackingObject.Radius = value;
    }
}
