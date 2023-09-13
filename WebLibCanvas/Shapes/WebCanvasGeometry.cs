using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILib.Extern.Web.Canvas;

namespace WebLibCanvas.Shapes;

public class WebCanvasGeometry
{
    protected WebCanvasGeometry(WebGeometry backingShape) => BackingGeometry = backingShape;

    public WebGeometry BackingGeometry { get; }
}

public class WebCanvasGeometry<TGeometry> : WebCanvasGeometry
    where TGeometry : WebGeometry
{
    protected WebCanvasGeometry(TGeometry backingObject) : base(backingObject) { }

    public TGeometry BackingObject => (TGeometry)BackingGeometry;
}
