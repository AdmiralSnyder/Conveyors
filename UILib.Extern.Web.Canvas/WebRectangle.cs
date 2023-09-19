using Blazor.Extensions.Canvas.Canvas2D;

namespace UILib.Extern.Web.Canvas;

public class WebRectangle : WebShape
{
    protected override async Task DrawAsyncVirtual(Canvas2DContext context)
    {
        //var oldStrokeStyle = context.StrokeStyle;
        //await context.SetStrokeStyleAsync($"rgb({StrokeColor.R},{StrokeColor.G},{StrokeColor.B})");
        await context.BeginPathAsync();

        await context.MoveToAsync(Location.X, Location.Y);
        await context.LineToAsync(Location.X + Width, Location.Y);
        await context.LineToAsync(Location.X + Width, Location.Y + Height);
        await context.LineToAsync(Location.X, Location.Y + Height);
        await context.LineToAsync(Location.X, Location.Y);
        
        // TODO move Dashing into property
        await context.SetLineDashAsync(new[] { 1f, 2f });
        await DrawAsyncInternal(context);
        await context.SetLineDashAsync(Array.Empty<float>());
    }

    public override bool ContainsPoint(Vector point) => false;

}
