using System.Drawing;
using UILib.Extern.Web.Canvas;
using UILib.Shapes;

namespace ConveyorLibWeb.Shapes;

public class WebCanvasEllipse: WebCanvasShape<WebEllipse>, IEllipse
{
    public WebCanvasEllipse(): base(new()) { }
}
