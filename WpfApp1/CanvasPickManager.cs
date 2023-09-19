using System.Windows.Controls;
using WpfLib;

namespace ConveyorApp;

public class CanvasSelectionManager : SelectionManager
{
    public override void UpdateBoundingBox(ISelectObject? selectObject) => Highlighter?.SetSelectObject(selectObject);
    
    // TODO Property
    public void SetCanvas(Canvas canvas) => Highlighter = UIHelpers.CreateObjectHighlighter(new WpfCanvasInfo() { Canvas = canvas });
}

public class CanvasPickManager : TargetObjectManager
{
    public override void UpdateBoundingBox(ISelectObject? selectObject) => Highlighter?.SetSelectObject(selectObject);

    // TODO Property
    public void SetCanvas(Canvas canvas) => Highlighter = UIHelpers.CreateObjectHighlighter(new WpfCanvasInfo() { Canvas = canvas });
}
