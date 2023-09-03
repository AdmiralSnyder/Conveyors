using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UILib;
using UILib.Shapes;
using WpfLib;
using WpfLib.Shapes;

namespace ConveyorApp;

public class CanvasObjectHighlighter(CanvasInfo CanvasInfo) : ObjectHighlighter
{
    public static CanvasObjectHighlighter Create(CanvasInfo canvas, ISelectObject selectObject, ObjectHighlightTypes objectHighlightType = ObjectHighlightTypes.Target)
    {
        CanvasObjectHighlighter result = new(canvas)
        {
            SelectObject = selectObject,
            HighlightType = objectHighlightType,
        };
        result.Highlight();
        return result;
    }
    private IRectangle? SelectionRect;

    protected override void Highlight()
    {
        if (SelectionRect is not null)
        {
            CanvasInfo.RemoveFromCanvas(SelectionRect);
        }
        if (SelectObject is null) return;

        var boundingRect = Maths.GetBoundingRectTopLeftSize(SelectObject.GetSelectionBoundsPoints());
        SelectionRect = new WpfRectangle(new()
        {
            Width = boundingRect.P2.X + 8,
            Height = boundingRect.P2.Y + 8,
            Stroke = HighlightType switch
            {
                ObjectHighlightTypes.None => null,
                ObjectHighlightTypes.Target => Brushes.Moccasin,
                ObjectHighlightTypes.Select => Brushes.Chartreuse,
                _ => null,
            },
            StrokeDashArray = new(new[] { 1d, 2d }),
            SnapsToDevicePixels = true,
            RadiusX = 2,
            RadiusY = 2,
        });
        SelectionRect.SetLocation(boundingRect.P1.Subtract((4, 4)));
        CanvasInfo.AddToCanvas(SelectionRect);
    }
}
