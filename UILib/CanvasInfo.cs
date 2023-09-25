using UILib.Shapes;

namespace UILib;

public interface ICanvasInfo
{ 
    TShape AddToCanvas<TShape>(TShape shape);
    TShape RemoveFromCanvas<TShape>(TShape shape);
    void BeginInvoke<T>(IShape shape, Action<T> action, T value);
    // TODO event
    void SelectionChanged();
}

public interface ICanvasInfo<TCanvas> : ICanvasInfo 
{
    public TCanvas Canvas { get; set; }
}

public abstract class CanvasInfo<TCanvas> : ICanvasInfo<TCanvas>
{
    // TODO this needs to go away
    public virtual object ResolveShape(object shape) => default;

    public TCanvas Canvas { get; set; }
    public IShapeProvider ShapeProvider { get; set; }
    public abstract TShape AddToCanvas<TShape>(TShape shape);
    public abstract void BeginInvoke<T>(IShape shape, Action<T> action, T value);
    public abstract TShape RemoveFromCanvas<TShape>(TShape shape);
    public virtual void SelectionChanged() { }
}