using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Web;
using UILib.Shapes;

namespace ConveyorLibWeb.Shapes;

public class WebCanvasShape<TShape> : WebCanvasShape
    where TShape : WebShape
{
    protected WebCanvasShape(TShape backingObject) : base(backingObject) { }
    public TShape BackingObject => (TShape)BackingShape;
}

public class WebCanvasShape : IShape, ITag
{
    public WebCanvasShape(WebShape backingShape) => BackingShape = backingShape;

    public object Tag { get; set; }
    public bool Visible { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
    public WebShape BackingShape { get; }
}
