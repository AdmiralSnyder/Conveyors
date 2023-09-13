using Blazor.Extensions.Canvas.Canvas2D;

namespace UILib.Extern.Web.Canvas;

public class WebPath : WebShape
{
    public WebGeometry Data { get; set; }

    protected override async Task DrawAsyncVirtual(Canvas2DContext context)
    {
        if (Data is WebPathGeometry wpg)
        {
            foreach (var figure in wpg.Figures)
            {
                await figure.DrawAsync(context);
            }
        }
        await DrawAsyncInternal(context);
    }
}
