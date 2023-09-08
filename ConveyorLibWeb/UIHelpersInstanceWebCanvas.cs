using ConveyorLibWeb.Shapes;
using PointDef;
using System;
using UILib;
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

    private TShape Modify<TShape, TArg>(TShape shape, Action<TShape, TArg> modifyFunc, TArg arg)
    {
        modifyFunc(shape, arg);
        return shape;
    }
}
