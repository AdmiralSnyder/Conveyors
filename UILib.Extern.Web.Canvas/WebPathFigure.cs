using Blazor.Extensions.Canvas.Canvas2D;
using UILib.Shapes;

namespace UILib.Extern.Web.Canvas;

public class WebArcSegment : WebPathSegment
{
    public WebArcSegment(Point center, double radius, double startAngleRad, double endAngleRad, bool antiClockwise, bool isStroked)
    {
        Center = center;
        Radius = radius;
        StartAngleRad = startAngleRad;
        EndAngleRad = endAngleRad;
        AntiClockwise = antiClockwise;
        IsStroked = isStroked;
    }

    public Point Center { get; }
    public double Radius { get; }
    public double StartAngleRad { get; }
    public double EndAngleRad { get; }
    public bool AntiClockwise { get; }
    public  bool IsStroked { get; }

    public override async Task DrawAsync(Canvas2DContext context)
    {
        if (IsStroked)
        {
            await context.BeginPathAsync();
            await context.ArcAsync(Center.X, Center.Y, Radius, StartAngleRad, EndAngleRad, AntiClockwise);
        }
        else
        {
            //await context.MoveToAsync(End.X, End.Y);
        }
    }
}

public class WebLineSegment : WebPathSegment
{
    private Vector End { get; set; }
    private bool IsStroked { get; set; }

    public WebLineSegment(Vector end, bool isStroked)
    {
        End = end;
        IsStroked = isStroked;
    }

    public override async Task DrawAsync(Canvas2DContext context)
    {
        if (IsStroked)
        {
            await context.LineToAsync(End.X, End.Y);
        }
        else
        {
            await context.MoveToAsync(End.X, End.Y);
        }
    }
}
public abstract class WebPathSegment
{
    public abstract Task DrawAsync(Canvas2DContext context);
}

public class WebPathFigure
{
    public Point StartPoint { get; set; }
    public List<WebPathSegment> Segments { get; } = new();

    public async Task DrawAsync(Canvas2DContext context)
    {
        await context.MoveToAsync(StartPoint.X, StartPoint.Y);
        foreach (var segment in Segments)
        {
            await segment.DrawAsync(context);
        }
    }
}
