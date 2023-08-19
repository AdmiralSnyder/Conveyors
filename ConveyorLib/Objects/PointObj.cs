using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using CoreLib.Definition;

namespace ConveyorLib.Objects;

public interface IPointObjDefinition
{
    public PointObjDefinition Definition { get; }
}

public class PointObj : ConveyorAppApplicationObject<PointObj, Shape, PointObjDefinition, Point>, IPointObjDefinition
{
     public override Vector[] GetSelectionBoundsPoints() => new[] { Definition.Point - (2, 2), Definition.Point + (2, 2)};

    protected override Shape GetShape() => CanvasInfo.ShapeProvider.CreatePoint(Definition.Point); 

}
