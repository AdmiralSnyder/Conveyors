using System.Windows.Media;
using CoreLib;
using UILib.Shapes;

namespace WpfLib.Shapes;

public class WpfLine : WpfShape<Line>, ILine
{
    public WpfLine (Line wpfShape) : base(wpfShape) { }

    public Brush Stroke 
    {
        get => BackingObject.Stroke;
        set => BackingObject.Stroke = value;
    }
    public double StrokeThickness 
    { 
        get => BackingObject.StrokeThickness; 
        set => BackingObject.StrokeThickness = value; 
    }
    public double X1 
    { 
        get => BackingObject.X1; 
        set => BackingObject.X1 = value;
    }
    public double Y1 
    { 
        get => BackingObject.Y1; 
        set => BackingObject.Y1 = value;
    }
    public double X2 
    { 
        get => BackingObject.X2; 
        set => BackingObject.X2 = value; 
    }
    public double Y2 
    {
        get => BackingObject.Y2; 
        set => BackingObject.Y2 = value; 
    }

    private System.Drawing.Color _StrokeColor;

    public System.Drawing.Color StrokeColor 
    {
        get => _StrokeColor;
        set => Func.Setter(ref _StrokeColor, value, () => BackingObject.Stroke = new SolidColorBrush(Color.FromArgb(value.A, value.R, value.G, value.B)));
    }
}
