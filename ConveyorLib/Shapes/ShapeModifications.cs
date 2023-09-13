using UILib.Behaviors;
using UILib.Shapes;
using static ConveyorLib.Shapes.ShapeSpecificationDefaults;

namespace ConveyorLib.Shapes;

public static class ShapeModifications
{
    public static TShape Selectable<TShape>(this TShape shape) where TShape : IShape
        => shape.WithSelectBehavior();

    public static TShape MarkAsTemporary<TShape>(this TShape shape) where TShape : IStroke =>
        shape.Modify(s => s.StrokeColor = TemporaryColor);

    public static TShape MarkAsDebug<TShape>(this TShape shape) where TShape : IStroke
        => shape.Modify(s => s.StrokeColor = DebugColor);

    public static TShape WithThinStroke<TShape>(this TShape shape) where TShape : IStroke
    => shape.Modify(s => s.StrokeThickness = ThinStrokeThickness);
}
