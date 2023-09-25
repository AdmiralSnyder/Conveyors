using UILib.Shapes;

namespace UILib;

public abstract class ObjectHighlighter
{
    private ISelectObject? _SelectObject;

    public ISelectObject? SelectObject
    {
        get => _SelectObject;
        set
        {
            if (value == null && _SelectObject == null) return;
            _SelectObject = value;
            Highlight();
        }
    }

    protected IRectangle? SelectionRect { get; set; }

    public void SetSelectObject(ISelectObject? selectObject) => SelectObject = selectObject;

    public ObjectHighlightTypes HighlightType { get; set; }

    public ICanvasInfo? CanvasInfo { get; init; }

    protected virtual void HighlightVirtual(Bounds locationSize) { }

    protected void Highlight()
    {
        if (SelectionRect is not null)
        {
            CanvasInfo?.RemoveFromCanvas(SelectionRect);
        }
        if (SelectObject is null) return;

        var locationSize = Maths.GetBoundingRectTopLeftSize(SelectObject.GetSelectionBoundsPoints());
        HighlightVirtual(locationSize);
        
        if (SelectionRect is not null)
        {
            SelectionRect.SetLocation(locationSize.Location.Subtract((4, 4)));

            CanvasInfo?.AddToCanvas(SelectionRect);
        }
    }
}
