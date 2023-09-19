using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConveyorLibWeb.Shapes;
using PointDef.twopoints;
using UILib.Extern.Web.Canvas;
using UILib.Shapes;

namespace WebLibCanvas.Shapes;

public class WebCanvasRectangle : WebCanvasShape<WebRectangle>, IRectangle
{
    public WebCanvasRectangle() : base(new()) { }

}
