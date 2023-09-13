using ConveyorLibWeb.Shapes;
using System;
using UILib;
using UILib.Extern.Web.Canvas;
using UILib.Shapes;

namespace ConveyorLibWeb;

public class UIHelpersInstanceWebCanvas : IUIHelpers
{
    public Vector GetSize(IShape shape) => ((WebCanvasShape)shape).BackingShape.GetSizeWeb();

    public TShape SetLocation<TShape>(TShape shape, Point location) where TShape : IShape
    {
        ((WebCanvasShape)(object)shape).BackingShape.SetLocationWeb(location);
        return shape;
    }

    public TShape SetLocation<TShape>(TShape shape, TwoPoints location) where TShape : IShape
    {
        ((WebCanvasShape<WebLine>)(object)shape).BackingObject.SetLocationWeb(location);
        return shape;
    }
}
