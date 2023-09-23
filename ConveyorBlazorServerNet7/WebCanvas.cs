using Microsoft.AspNetCore.Components.Web;
using UILib.Extern.Web.Canvas;

namespace ConveyorBlazorServerNet7;

public class WebCanvas
{
    public List<WebShape> Children { get; } = new();

    public List<WebShape> TempChildren { get; } = new();

    public event Action<EventArgs> MouseDown;
    public event Action<EventArgs> MouseMove;

    public void DoMouseClick(MouseEventArgs args) => MouseDown?.Invoke(args);

    public void DoMouseMove(MouseEventArgs args) => MouseMove?.Invoke(args);
}
