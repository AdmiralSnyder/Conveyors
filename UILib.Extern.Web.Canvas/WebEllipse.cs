using System.Drawing;
using Blazor.Extensions.Canvas.Canvas2D;

namespace UILib.Extern.Web.Canvas;

public class WebEllipse : WebShape
{
    private double Radius => Width / 2;

    protected override async Task DrawAsyncVirtual(Canvas2DContext context)
    {
        await context.BeginPathAsync();
        await context.ArcAsync(Location.X + Radius, Location.Y + Radius, Radius, 0, 2 * Math.PI);
        await DrawAsyncInternal(context);
    }

    public override bool ContainsPoint(Point point) => Maths.PointIsInCircle(point, new(Location + (Width / 2, Height / 2), Radius));
}
