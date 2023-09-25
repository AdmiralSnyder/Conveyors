using System.Drawing;
using Blazor.Extensions.Canvas.Canvas2D;
using CoreLib;
using PointDef.twopoints;

namespace UILib.Extern.Web.Canvas;

public class WebLine : WebShape
{
    private TwoPoints<Vector> fromTo;

    public TwoPoints FromTo 
    {
        get => fromTo;
        set => Func.Setter(ref fromTo, value, UpdateLocationAndSize);
    }

    private void UpdateLocationAndSize(TwoPoints definingPoints)
    {
        var bounds = Maths.GetBounds(definingPoints);
        Location = bounds.Location;
        Width = bounds.Size.X;
        Height = bounds.Size.Y;
    }

    protected override async Task DrawAsyncVirtual(Canvas2DContext context)
    {
        //var oldStrokeStyle = context.StrokeStyle;
        //await context.SetStrokeStyleAsync($"rgb({StrokeColor.R},{StrokeColor.G},{StrokeColor.B})");
        await context.BeginPathAsync();
        await context.MoveToAsync(FromTo.P1.X, FromTo.P1.Y);
        await context.LineToAsync(FromTo.P2.X, FromTo.P2.Y);
        await DrawAsyncInternal(context);
        //await context.SetStrokeStyleAsync(oldStrokeStyle);
    }

    public override bool ContainsPoint(Vector point) => false;
}
