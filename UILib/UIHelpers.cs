using CoreLib.Definition;
using PointDef;
using UILib.Shapes;

namespace UILib;

public interface IUIHelpers
{
    Vector GetSize(IShape shape);
    TShape SetLocation<TShape>(TShape shape, Point location) where TShape : IShape;
    TShape SetLocation<TShape>(TShape shape, TwoPoints location) where TShape : IShape;

    bool TryGetLocation<TShape>(TShape shape, out Point location) where TShape : IShape;

    ObjectHighlighter CreateObjectHighlighter(ICanvasInfo canvasInfo, ISelectObject? selectObject, ObjectHighlightTypes highlightTypes);
    
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

    public static ObjectHighlighter CreateObjectHighlighter(ICanvasInfo canvasInfo, ISelectObject? selectObject = null, ObjectHighlightTypes highlightTypes = ObjectHighlightTypes.Target)
        => Instance.CreateObjectHighlighter(canvasInfo, selectObject, highlightTypes);
}
