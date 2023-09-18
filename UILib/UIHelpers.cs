using CoreLib.Definition;
using PointDef;
using UILib.Shapes;

namespace UILib;

public interface IUIHelpers
{
    public Vector GetSize(IShape shape);
    public TShape SetLocation<TShape>(TShape shape, Point location) where TShape : IShape;
    public TShape SetLocation<TShape>(TShape shape, TwoPoints location) where TShape : IShape;

    public bool TryGetLocation<TShape>(TShape shape, out Point location) where TShape : IShape;
    
}

public static class UIHelpers
{
    public static IUIHelpers Instance { get; set; }

    public static Vector GetSize(IShape shape) 
        => Instance.GetSize(shape);

    public static TShape SetLocation<TShape>(TShape shape, Point location) where TShape : IShape
        => Instance.SetLocation(shape, location);

    public static TShape SetLocation<TShape>(TShape shape, TwoPoints location) where TShape : IShape 
        => Instance.SetLocation(shape, location);

    public static bool TryGetLocation<TShape>(TShape shape, out Point location) where TShape : IShape 
        => Instance.TryGetLocation(shape, out location);
}
