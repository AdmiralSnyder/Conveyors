using UILib.Shapes;

namespace WpfLib.Shapes;

public class WpfPath : WpfShape<Path>, IPath
{
    public WpfPath (Path wpfShape) : base(wpfShape) { }
}
