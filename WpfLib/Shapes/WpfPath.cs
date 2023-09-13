using CoreLib;
using UILib.Shapes;

namespace WpfLib.Shapes;

public class WpfPath : WpfShape<Path>, IPath
{
    public WpfPath () : base(new()) { }
    private IPathGeometry _Geometry;
    public IPathGeometry Geometry 
    {
        get => _Geometry; 
        set => Func.Setter(ref _Geometry, value, geo => BackingObject.Data = ((WpfPathGeometry)geo).BackingGeometry); 
    }
}
