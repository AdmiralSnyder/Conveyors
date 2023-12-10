using System.Drawing;
using Blazor.Extensions.Canvas.Canvas2D;
using CoreLib;

namespace UILib.Extern.Web.Canvas;

public abstract class WebShape : IBounded
{
    public async Task DrawAsync(Canvas2DContext context)
    {
        if (!Visible) return;
        await DrawAsyncVirtual(context);
    }

    public abstract bool ContainsPoint(Point point);

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

    private Dictionary<MouseActions, Delegate> MouseActions = [];
    public void AddMouseAction(MouseActions mouseAction, Delegate action) => this.MouseActions[mouseAction] = action;
    public bool TryGetMouseAction(MouseActions mouseAction, out Delegate action) => this.MouseActions.TryGetValue(mouseAction, out action);

    public bool Visible { get; set; } = true;
    public double Height { get; set; }
    public double Width { get; set; }
    public Point Location { get; set; }
    public Color? Fill { get; set; }
    public Color? StrokeColor { get; set; } = Color.Black;
    public double StrokeThickness { get; set; } = 1;

    public Bounds Bounds => new(Location, (Width, Height));
}
