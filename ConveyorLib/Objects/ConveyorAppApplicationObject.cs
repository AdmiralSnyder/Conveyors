using System.Windows;
using System.Windows.Controls;

namespace ConveyorLib.Objects;

public abstract class ConveyorAppApplicationObject<TShape> : CanvasableObject<ConveyorCanvasInfo, Canvas, ConveyorAppApplication, TShape>
    where TShape : FrameworkElement
{
    protected override void AddToCanvasVirtual(TShape shape) => Canvas.Children.Add(shape);
    protected override void SetTag(TShape shape, object tag) => shape.Tag = tag;
}
