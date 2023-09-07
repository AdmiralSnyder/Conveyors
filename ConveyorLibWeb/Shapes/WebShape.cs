using Blazor.Extensions.Canvas.Canvas2D;

namespace ConveyorLibWeb.Shapes;

public abstract class WebShape
{
    public async Task DrawAsync(Canvas2DContext context)
    {
        if (!Visible) return;
        await DrawAsyncVirtual(context);
    }

    protected abstract Task DrawAsyncVirtual(Canvas2DContext context);

    public bool Visible { get; set; } = true;
    public double Height { get; set; }
    public double Width { get; set; }
}
