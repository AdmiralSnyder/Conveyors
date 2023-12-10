using Microsoft.AspNetCore.Components.Web;
using UILib.Extern.Web.Canvas;

namespace ConveyorBlazorServerNet7;

public class WebCanvas
{
    public QuadTree<WebShape> QuadTree { get; } = new(new((-1000, -1000), (2000, 2000)), 4);
    private List<WebShape> _Children { get; } = [];
    public IEnumerable<WebShape> Children => _Children;

    public List<WebShape> TempChildren { get; } = [];

    public void AddChild(WebShape child)
    {
        _Children.Add(child);
        QuadTree.Add(child);
    }

    public void RemoveChild(WebShape child)
    {
        _Children.Remove(child);
        QuadTree.Remove(child);
    }

    public event Action<EventArgs> MouseDown;
    public event Action<EventArgs> MouseMove;

    public void DoMouseClick(MouseEventArgs args) => MouseDown?.Invoke(args);

    public void DoMouseMove(MouseEventArgs args) => MouseMove?.Invoke(args);
}
