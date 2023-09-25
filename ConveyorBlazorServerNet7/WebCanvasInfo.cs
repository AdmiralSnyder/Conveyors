using System.Drawing;
using ConveyorAutomationLib;
using ConveyorLib;
using ConveyorLib.Objects;
using ConveyorLib.Shapes;
using ConveyorLibWeb.Shapes;
using UILib.Extern.Web.Canvas;
using UILib.Shapes;
using WebLibCanvas.Shapes;

namespace ConveyorBlazorServerNet7;

public class WebCanvasInfo : CanvasInfo<WebCanvas>, IConveyorCanvasInfo
{
    public override object ResolveShape(object shape)
    {
        if (ShapeResolver.TryGetValue((WebShape)shape, out var obj))
        {
            return obj;
        }
        return default;
    }

    // TODO this is dirty
    private Dictionary<WebShape, object> ShapeResolver = new();

    public override TShape AddToCanvas<TShape>(TShape shape)
    {
        Canvas.AddChild(((WebCanvasShape)(object)shape).BackingShape);

        ShapeResolver.Add(((WebCanvasShape)(object)shape).BackingShape, shape);
        return shape;
    }

    public TShape AddToCanvasTemporary<TShape>(TShape shape)
    {
        Canvas.TempChildren.Add(((WebCanvasShape)(object)shape).BackingShape);
        return shape;
    }

    private List<WebCanvasShape> SelectionShapes = new();

    public override void SelectionChanged()
    {
        base.SelectionChanged();

        ClearSelection();
        foreach (ISelectObject selectedObject in AutoRoot.GetSelectObjects())
        {
            var bounds = Maths.GetBoundingRectTopLeftSize(selectedObject.GetSelectionBoundsPoints());

            var widthHeight = bounds.Size.Add((8, 8));
            var rect = new WebCanvasRectangle()
            {
                Width = widthHeight.X,
                Height = widthHeight.Y,
                StrokeColor = Color.BlueViolet,
            }.SetLocation(bounds.Location - (4, 4));
            SelectionShapes.Add(AddToCanvasTemporary(rect));
        }
    }

    public void ClearSelection()
    {
        foreach (var shape in SelectionShapes)
        {
            RemoveFromCanvas(shape);
        }
        SelectionShapes.Clear();
    }

    public override void BeginInvoke<T>(IShape shape, Action<T> action, T value)
    {
        throw new NotImplementedException();
    }

    IConveyorShapeProvider IConveyorCanvasInfo.ShapeProvider
    {
        get => (IConveyorShapeProvider)ShapeProvider;
        set => ShapeProvider = value;
    }

    public override TShape RemoveFromCanvas<TShape>(TShape shape)
    {
        Canvas.RemoveChild(((WebCanvasShape)(object)shape).BackingShape);
        Canvas.TempChildren.Remove(((WebCanvasShape)(object)shape).BackingShape);

        ShapeResolver.Remove(((WebCanvasShape)(object)shape).BackingShape);
        return shape;
    }
}
