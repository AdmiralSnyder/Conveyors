using System.Windows;
using WpfLib;

namespace ConveyorLib;

class ItemTextAdorner : TextAdorner<Item>
{
    public ItemTextAdorner(UIElement adornedElement) : base(adornedElement) { }
}
