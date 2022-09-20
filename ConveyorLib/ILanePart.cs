using System.Collections.Generic;

namespace ConveyorLib;

public interface ILanePart
{
    double BeginLength { set; }
    double Length { get; }
    double EndLength { get; }
    Point GetPointAbsolute(double length, bool overshoot = false);
    LinkedListNode<ILanePart> ElementsNode { get; }
}
