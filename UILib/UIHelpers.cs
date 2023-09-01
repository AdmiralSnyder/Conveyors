using PointDef;

namespace UILib;

public interface IUIHelpers
{
    public V2d GetSize<TShape>(TShape shape);
    public TShape SetLocation<TShape>(TShape shape, Point location);
    public TShape SetLocation<TShape>(TShape shape, TwoPoints location);
}
public static class UIHelpers
{
    public static IUIHelpers Instance { get; set; }

    public static V2d GetSize<TShape>(TShape shape) => Instance.GetSize(shape);
    public static TShape SetLocation<TShape>(TShape shape, Point location) => Instance.SetLocation(shape, location);
    public static TShape SetLocation<TShape>(TShape shape, TwoPoints location) => Instance.SetLocation(shape, location);
}
