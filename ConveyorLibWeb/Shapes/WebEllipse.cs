using Blazor.Extensions.Canvas.Canvas2D;

namespace ConveyorLibWeb.Shapes;

public class WebEllipse : WebShape
{
    public Point Center { get; set; }
    public double Radius { get; set; }
    protected override async Task DrawAsyncVirtual(Canvas2DContext context)
    {
        await context.BeginPathAsync();
        await context.ArcAsync(Center.X, Center.Y, Radius, 0, 2 * Math.PI);
        await context.StrokeAsync();
    }
}
