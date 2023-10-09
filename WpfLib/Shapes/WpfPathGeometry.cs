using System;
using System.Windows.Media;
using UILib.Shapes;

namespace WpfLib.Shapes;

public class WpfPathGeometry : WpfGeometry<PathGeometry>, IPathGeometry
{
    public WpfPathGeometry() : base(new()) { }

    public void AddArcFigure(Vector prevEnd, Vector nextStart, double radius, double degrees, bool isLargeArc, ArcSweepDirections sweepDirection)
    => BackingObject.Figures.Add(new()
    {
        StartPoint = prevEnd.AsPoint(),
        Segments = { new ArcSegment(nextStart.AsPoint(), new(radius, radius), degrees, isLargeArc, (SweepDirection)sweepDirection, true) }
    });

    public void AddLineFigure(Vector prevEnd, Vector nextStart)
    {
        BackingObject.Figures.Add(new()
        {
            StartPoint = prevEnd.AsPoint(),
            Segments = { new LineSegment(nextStart.AsPoint(), true) }
        });
    }

    public void ClearFigures() => BackingObject.Figures.Clear();
}
