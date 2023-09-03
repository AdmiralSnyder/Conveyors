using PointDef;
using UILib.Shapes;

namespace UILib;

public interface IUIHelpers
{
    public V2d GetSize(IShape shape);
    public TShape SetLocation<TShape>(TShape shape, Point location) where TShape : IShape;
    public TShape SetLocation<TShape>(TShape shape, TwoPoints location) where TShape : IShape;
}
public static class UIHelpers
{
    public static IUIHelpers Instance { get; set; }

    public static V2d GetSize(IShape shape) 
        => Instance.GetSize(shape);

    public static TShape SetLocation<TShape>(TShape shape, Point location) where TShape : IShape
        => Instance.SetLocation(shape, location);

    public static TShape SetLocation<TShape>(TShape shape, TwoPoints location) where TShape : IShape 
        => Instance.SetLocation(shape, location);
}
