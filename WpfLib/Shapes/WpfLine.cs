using System.Windows.Media;
using CoreLib;
using UILib.Shapes;

namespace WpfLib.Shapes;

public class WpfLine : WpfShape<Line>, ILine
{
    public WpfLine () : base(new Line()) { }

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
}
