using System.Drawing;
using UILib.Extern.Web.Canvas;
using UILib.Shapes;

namespace ConveyorLibWeb.Shapes;

public class WebCanvasShape<TShape> : WebCanvasShape
    where TShape : WebShape
{
    protected WebCanvasShape(TShape backingObject) : base(backingObject) { }
    public TShape BackingObject => (TShape)BackingShape;
}

public class WebCanvasShape : IShape, ITag
{
    public WebCanvasShape(WebShape backingShape) => BackingShape = backingShape;

    public object Tag { get; set; }

    public WebShape BackingShape { get; }

    public double Height
    {
        get => BackingShape.Height; 
        set => BackingShape.Height = value; 
    }
    public double Width
    {
        get => BackingShape.Width;
        set => BackingShape.Width = value;
    }
    public bool Visible
    {
        get => BackingShape.Visible;
        set => BackingShape.Visible = value;
    }

    public Color? FillColor
    {
        get => BackingShape.Fill;
        set => BackingShape.Fill = value;
    }
    public Color? StrokeColor
    {
        get => BackingShape.StrokeColor;
        set => BackingShape.StrokeColor = value;
    }

    public double StrokeThickness
    {
        get => BackingShape.StrokeThickness;
        set => BackingShape.StrokeThickness = value;
    }
}
