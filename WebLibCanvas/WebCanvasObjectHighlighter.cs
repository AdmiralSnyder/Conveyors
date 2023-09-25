using System.Drawing;
using UILib;
using WebLibCanvas.Shapes;

namespace WebLibCanvas;

public class WebCanvasObjectHighlighter : ObjectHighlighter
{
    public static WebCanvasObjectHighlighter Create(ICanvasInfo canvasInfo, ISelectObject selectObject, ObjectHighlightTypes objectHighlightType = ObjectHighlightTypes.Target)
    {
        WebCanvasObjectHighlighter result = new()
        {
            CanvasInfo = canvasInfo,
            SelectObject = selectObject,
            HighlightType = objectHighlightType,
        };

        result.Highlight();
        return result;
    }

    protected override void HighlightVirtual(Bounds locationSize)
    {
        // TODO get this from the ShapeProvider
        SelectionRect = new WebCanvasRectangle()
        {
            Width = locationSize.Size.X + 8,
            Height = locationSize.Size.Y + 8,
            StrokeColor = HighlightType switch
            {
                ObjectHighlightTypes.None => null,
                ObjectHighlightTypes.Target => Color.Moccasin,
                ObjectHighlightTypes.Select => Color.Chartreuse,
                _ => null,
            },
        }.SetLocation(locationSize.Location - (4, 4));
    }
}
