using System.Windows;
using UILib.Shapes;

namespace WpfLib.Shapes;

public class WpfShape : IShape, ITag
{
    protected WpfShape(Shape backingShape) => BackingShape = backingShape;

    public Shape BackingShape { get; }

    public bool Visible
    {
        get => BackingShape.Visibility == Visibility.Visible;
        set => BackingShape.Visibility = value ? Visibility.Visible : Visibility.Hidden;
    }

    public object Tag 
    {
        get => BackingShape.Tag;
        set => BackingShape.Tag = value;
    }

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

    public System.Drawing.Color? FillColor 
    {
        get => BackingShape.Fill.GetBrushColor();
        set => BackingShape.Fill = value.AsBrush(); 
    }
    public System.Drawing.Color? StrokeColor
    {
        get => BackingShape.Stroke.GetBrushColor();
        set => BackingShape.Stroke = value.AsBrush();
    }

    public double StrokeThickness 
    {
        get => BackingShape.StrokeThickness; 
        set => BackingShape.StrokeThickness = value; 
    }
}

public class WpfShape<TShape> : WpfShape
    where TShape : Shape
{
    protected WpfShape(TShape backingObject) : base(backingObject) { }
    public TShape BackingObject => (TShape)BackingShape;
}
