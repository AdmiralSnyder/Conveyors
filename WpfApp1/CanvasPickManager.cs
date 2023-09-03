using System.Windows.Controls;
using UILib;

namespace ConveyorApp;

public class CanvasSelectionManager : SelectionManager
{
    public override void UpdateBoundingBox(ISelectObject? selectObject) => Highlighter?.SetSelectObject(selectObject);

    // TODO Property
    public void SetCanvas(Canvas canvas) => Highlighter = new CanvasObjectHighlighter(new() { Canvas = canvas })
    {
        HighlightType = ObjectHighlightTypes.Select
    };
}

public class CanvasPickManager : TargetObjectManager
{
    public override void UpdateBoundingBox(ISelectObject? selectObject) => Highlighter?.SetSelectObject(selectObject);

    // TODO Property
    public void SetCanvas(Canvas canvas) => Highlighter = new CanvasObjectHighlighter(new() { Canvas = canvas })
    {
        HighlightType = ObjectHighlightTypes.Target
    };
}
