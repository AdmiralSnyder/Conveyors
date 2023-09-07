using System.Drawing;
using Blazor.Extensions.Canvas.Canvas2D;

namespace ConveyorLibWeb.Shapes;

public class WebLine : WebShape
{
    public Color StrokeColor { get; set; }
    public double StrokeThickness { get; set; }
    public TwoPoints FromTo { get; set; }

    protected override async Task DrawAsyncVirtual(Canvas2DContext context)
    {
        //var oldStrokeStyle = context.StrokeStyle;
        //await context.SetStrokeStyleAsync($"rgb({StrokeColor.R},{StrokeColor.G},{StrokeColor.B})");
        await context.BeginPathAsync();
        await context.MoveToAsync(FromTo.P1.X, FromTo.P1.Y);
        await context.LineToAsync(FromTo.P2.X, FromTo.P2.Y);
        await context.StrokeAsync();
        //await context.SetStrokeStyleAsync(oldStrokeStyle);
    }
}
