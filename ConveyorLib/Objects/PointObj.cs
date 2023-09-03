using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Definition;
using UILib.Shapes;

namespace ConveyorLib.Objects;

public interface IPointObjDefinition
{
    public PointObjDefinition Definition { get; }
}

public class PointObj : ConveyorAppApplicationObject<PointObj, IShape, PointObjDefinition, Point>, IPointObjDefinition
{
    protected override IShape GetShape() => CanvasInfo.ShapeProvider.CreatePoint(Definition.Point); 
    public override Vector[] GetSelectionBoundsPoints() => new[] { Definition.Point - (2, 2), Definition.Point + (2, 2)};
}
