using System.Windows;
using UILib.Shapes;

namespace WpfLib.Shapes;

public class WpfShape : IShape, ITag
{
    protected WpfShape(Shape backingShape) => BackingShape = backingShape;

    public Shape BackingShape { get; }
    public Visibility Visibility 
    {
        get => BackingShape.Visibility;
        set => BackingShape.Visibility = value;
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
}


public class WpfShape<TShape> : WpfShape
    where TShape : Shape
{
    protected WpfShape(TShape backingObject) : base(backingObject) { }
    public TShape BackingObject => (TShape)BackingShape;
}
