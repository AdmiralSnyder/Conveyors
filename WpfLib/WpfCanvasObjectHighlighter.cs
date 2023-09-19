using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UILib.Shapes;
using WpfLib;
using WpfLib.Shapes;
using CoreLib.Maths;

namespace WpfLib;

public class WpfCanvasObjectHighlighter : ObjectHighlighter
{
    public static WpfCanvasObjectHighlighter Create(ICanvasInfo canvasInfo, ISelectObject? selectObject, ObjectHighlightTypes objectHighlightType = ObjectHighlightTypes.Target)
    {
        WpfCanvasObjectHighlighter result = new()
        {
            CanvasInfo = canvasInfo,
            SelectObject = selectObject,
            HighlightType = objectHighlightType,
        };

        result.Highlight();
        return result;
    }

    protected override void HighlightVirtual(TwoPoints locationSize)
    {
        // TODO get this from the ShapeProvider
        var rect = new WpfRectangle
        {
            Width = locationSize.P2.X + 8,
            Height = locationSize.P2.Y + 8,
            StrokeColor= HighlightType switch
            {
                ObjectHighlightTypes.None => null,
                ObjectHighlightTypes.Target => System.Drawing.Color.Moccasin,
                ObjectHighlightTypes.Select => System.Drawing.Color.Chartreuse,
                _ => null,
            },
            
        };
        rect.BackingObject.StrokeDashArray = new(new[] { 1d, 2d });
        rect.BackingObject.SnapsToDevicePixels = true;
        rect.BackingObject.RadiusX = 2;
        rect.BackingObject.RadiusY = 2;
        SelectionRect = rect;
    }
}
