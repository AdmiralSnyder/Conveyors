using System.Windows.Shapes;
using WpfLib;
using System.Windows.Media;
using SDColor = System.Drawing.Color;
using System.Windows.Input;
using System.Windows.Documents;
using UILib.Shapes;
using WpfLib.Shapes;
using UILib.Behaviors;
using ConveyorLib.Shapes;

namespace ConveyorLib.Wpf;

public class WpfConveyorShapeProvider : ConveyorShapeProvider<WpfEllipse, WpfLine, WpfPath>, IConveyorShapeProvider
{
    public override IEllipse CreatePointMoveCircle(Point location, Action<IShape> leftClickAction)
        => base.CreatePointMoveCircle(location, leftClickAction)
        .Modify(result => ((WpfEllipse)result).BackingObject.Cursor = Cursors.Hand);

    public override void AddAdornedShapeLayer(IShape shape)
    {
        var layer = AdornerLayer.GetAdornerLayer(((WpfShape)shape).BackingShape);
        layer.Add(new ItemTextAdorner(((WpfShape)shape).BackingShape));
    }
}