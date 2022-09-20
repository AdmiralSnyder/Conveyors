namespace UILib;

public interface ICanvasable<TCanvasInfo>
    where TCanvasInfo : CanvasInfo
{
    void AddToCanvas(TCanvasInfo canvasInfo);
}
