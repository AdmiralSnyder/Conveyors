using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UILib;
using WpfLib;

namespace ConveyorApp;

public class CanvasSelectionManager(Canvas canvas) 
    : SelectionManager(new CanvasObjectHighlighter(canvas) 
    { 
        HighlightType = ObjectHighlightTypes.Select 
    })
{
    public override void UpdateBoundingBox(ISelectObject? selectObject) => Highlighter.SelectObject = selectObject;
}

public class CanvasPickManager(Canvas canvas) 
    : PickManager(new CanvasObjectHighlighter(canvas)
    { 
        HighlightType = ObjectHighlightTypes.Pick 
    })
{
    public override void UpdateBoundingBox(ISelectObject? selectObject) => Highlighter.SelectObject = selectObject;
}
