namespace ConveyorLib.Objects;

public interface ICanAddToCanvas<TCanvasInfo>
    where TCanvasInfo : ICanvasInfo
{
    void AddToCanvas(TCanvasInfo canvasInfo);
}


public abstract class CanvasableObject<TCanvasInfo, TApplication, TShape> 
    : ApplicationObject<TApplication>, ICanAddToCanvas<TCanvasInfo>
    where TApplication : IApplication
    where TCanvasInfo : ICanvasInfo
{
    protected TShape Shape { get; private set; }

    protected TCanvasInfo CanvasInfo { get; private set; }
    protected abstract void SetTag(TShape shape, object tag);

    protected abstract TShape GetShape();
    protected abstract void AddToCanvasVirtual(TShape shape);
    
    public void AddToCanvas(TCanvasInfo canvasInfo)
    {
        CanvasInfo = canvasInfo;
        Shape = GetShape();
        SetTag(Shape, this);
        AddToCanvasVirtual(Shape);
    }

}
