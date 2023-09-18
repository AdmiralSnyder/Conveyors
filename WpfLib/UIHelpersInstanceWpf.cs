using PointDef;
using System;
using UILib.Shapes;
using WpfLib.Shapes;

namespace WpfLib;

public class UIHelpersInstanceWpf : IUIHelpers
{
    public bool TryGetLocation<TShape>(TShape shape, out Vector location) where TShape : IShape
    {
        if (shape is WpfShape wpfShape && wpfShape.BackingShape is Shape shp && shp.TryGetLocationWpf(out location))
        {
            return true;
        }
        else
        {
            location = default;
            return false;
        }
    }

    public V2d GetSize(IShape shape) => ((WpfShape)shape).BackingShape.GetSizeWpf();

    public TShape SetLocation<TShape>(TShape shape, Point location) where TShape : IShape 
    {
        ((WpfShape)(object)shape).BackingShape.SetLocationWpf(location);
        return shape;
    }

    public TShape SetLocation<TShape>(TShape shape, TwoPoints location)where TShape : IShape
    {
        ((WpfShape<Line>)(object)shape).BackingObject.SetLocationWpf(location);
        return shape;
    }

    private TShape Modify<TShape, TArg>(TShape shape, Action<TShape, TArg> modifyFunc, TArg arg)
    {
        modifyFunc(shape, arg);
        return shape;
    }
}
