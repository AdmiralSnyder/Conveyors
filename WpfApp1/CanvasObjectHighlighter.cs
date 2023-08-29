using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UILib;
using WpfLib;

namespace ConveyorApp;

public class CanvasObjectHighlighter(Canvas Canvas) : ObjectHighlighter
{
    public static CanvasObjectHighlighter Create(Canvas canvas, ISelectObject selectObject, ObjectHighlightTypes objectHighlightType = ObjectHighlightTypes.Target)
    {
        CanvasObjectHighlighter result = new(canvas)
        {
            SelectObject = selectObject,
            HighlightType = objectHighlightType,
        };
        result.Highlight();
        return result;
    }
    private Rectangle? SelectionRect;

    protected override void Highlight()
    {
        if (SelectionRect is not null)
        {
            Canvas.Children.Remove(SelectionRect);
        }
        if (SelectObject is null) return;

        var boundingRect = Maths.GetBoundingRectTopLeftSize(SelectObject.GetSelectionBoundsPoints());
        SelectionRect = new()
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
        };
        SelectionRect.SetLocation(boundingRect.P1.Subtract((4, 4)));
        Canvas.Children.Add(SelectionRect);
    }
}
