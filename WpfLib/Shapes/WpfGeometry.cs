using System.Windows.Media;

namespace WpfLib.Shapes;

public class WpfGeometry
{
    protected WpfGeometry(Geometry backingShape) => BackingGeometry = backingShape;

    public Geometry BackingGeometry { get; }
}

public class WpfGeometry<TGeometry> : WpfGeometry
    where TGeometry : Geometry
{
    protected WpfGeometry(TGeometry backingObject) : base(backingObject) { }

    public TGeometry BackingObject => (TGeometry)BackingGeometry;
}