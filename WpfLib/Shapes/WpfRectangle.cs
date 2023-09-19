using UILib.Shapes;

namespace WpfLib.Shapes;

public class WpfRectangle : WpfShape<Rectangle>, IRectangle
{
    public WpfRectangle () : base(new()) { }
}
