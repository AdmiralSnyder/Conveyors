using System.Windows;
using ConveyorLib.Objects.Conveyor;
using WpfLib;

namespace ConveyorLib;

class ItemTextAdorner : TextAdorner<Item>
{
    public ItemTextAdorner(UIElement adornedElement) : base(adornedElement) { }
}
