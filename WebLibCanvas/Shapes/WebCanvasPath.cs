using System;
using CoreLib;
using UILib.Extern.Web.Canvas;
using UILib.Shapes;
using WebLibCanvas.Shapes;

namespace ConveyorLibWeb.Shapes;

public class WebCanvasPath : WebCanvasShape<WebPath>, IPath
{
    public WebCanvasPath () : base(new()) { }
    private IPathGeometry _Geometry;
    public IPathGeometry Geometry 
    {
        get => _Geometry; 
        set => Func.Setter(ref _Geometry, value, geo => BackingObject.Data = ((WebCanvasPathGeometry)geo).BackingGeometry); 
    }
}
