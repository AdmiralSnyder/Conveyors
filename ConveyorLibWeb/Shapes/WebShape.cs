﻿using System.Drawing;
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

    protected async Task DrawStroke(Canvas2DContext context)
    {
        if (StrokeColor.HasValue)
        {
            var oldStroke = context.StrokeStyle;
            var oldLineWidth = context.LineWidth;
            await context.SetLineWidthAsync((float)StrokeThickness);
            await context.SetStrokeStyleAsync(StrokeColor.Value.ToStrokeStyle());
            await context.StrokeAsync();
            await context.SetLineWidthAsync(oldLineWidth);
            await context.SetStrokeStyleAsync(oldStroke);
        }
    }

    protected async Task DrawFill(Canvas2DContext context)
    {
        if (Fill.HasValue)
        {
            var oldFill = context.FillStyle;
            await context.SetFillStyleAsync(Fill.Value.ToFillStyle());
            await context.FillAsync();
            await context.SetFillStyleAsync(oldFill);
        }
    }

    protected async Task DrawAsyncInternal(Canvas2DContext context)
    {
        await DrawFill(context);
        await DrawStroke(context);
    }

    public bool Visible { get; set; } = true;
    public double Height { get; set; }
    public double Width { get; set; }
    public Vector Location { get; internal set; }
    public Color? Fill { get; set; }
    public Color? StrokeColor { get; set; } = Color.Black;
    public double StrokeThickness { get; set; } = 1;
}
