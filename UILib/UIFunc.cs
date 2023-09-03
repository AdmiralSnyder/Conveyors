using PointDef;
using UILib.Shapes;

namespace UILib;

public static class UIFunc
{
    public static V2d GetSize<TShape>(this TShape shape) where TShape : IShape => UIHelpers.GetSize(shape);
    public static TShape SetCenterLocation<TShape>(this TShape shape, Point location) where TShape : IShape
        => UIHelpers.SetLocation(shape, (location.Subtract(UIHelpers.GetSize(shape).Divide(2))));

    public static TShape SetLocation<TShape>(this TShape shape, Point location) where TShape : IShape
        => UIHelpers.SetLocation(shape, location);

    public static TShape SetLocation<TShape>(this TShape shape, TwoPoints location) where TShape : IShape
        => UIHelpers.SetLocation(shape, location);

}
