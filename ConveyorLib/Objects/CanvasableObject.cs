namespace ConveyorLib.Objects;

public abstract class CanvasableObject<TCanvasInfo, TCanvas, TApplication, TShape> : ApplicationObject<TApplication>
    where TApplication : IApplication
    where TCanvasInfo : CanvasInfo<TCanvas>
    where TCanvas : class
{
    protected TShape Shape { get; private set; }

    protected TCanvas? Canvas { get; private set; }
    protected TCanvasInfo CanvasInfo { get; private set; }
    protected abstract void SetTag(TShape shape, object tag);

    protected abstract TShape GetShape();
    protected abstract void AddToCanvasVirtual(TShape shape);
    
    public void AddToCanvas(TCanvasInfo canvasInfo)
    {
        Canvas = canvasInfo.Canvas;
        CanvasInfo = canvasInfo;
        Shape = GetShape();
        SetTag(Shape, this);
        AddToCanvasVirtual(Shape);
    }

}
