namespace UILib;

public interface ICanvasable<TCanvasInfo>
    where TCanvasInfo : ICanvasInfo
{
    void AddToCanvas(TCanvasInfo canvasInfo);
}
