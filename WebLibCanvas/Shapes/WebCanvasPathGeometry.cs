using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Definition;
using UILib.Extern.Web.Canvas;
using UILib.Shapes;

namespace WebLibCanvas.Shapes;

public class WebCanvasPathGeometry : WebCanvasGeometry<WebPathGeometry>, IPathGeometry
{
    public WebCanvasPathGeometry() : base(new()) { }

    public void AddArcFigure(Vector from, Vector to, double radius, double degrees, bool isLargeArc, ArcSweepDirections sweepDirection)
    {
        CircleDefinition cFrom = new(from, radius);
        LineDefinition fromTo = new((from, to));
        var mid = Maths.MidPoint((from, to));
        LineDefinition ortho = new(mid, fromTo.Vector.Orthogonal());
        var points = Maths.CircleCrossesLine(cFrom, ortho);
        var point = points[sweepDirection == ArcSweepDirections.Clockwise ? 0 : 1]; // TODO maybe find better condition
        var startAngle = new Vector(point, from).Angle().Radians;
        var endAngle = new Vector(point, to).Angle().Radians;

        bool anticlockwise = true;
        var radians = degrees.Degrees().Radians;
        if (degrees < 0)
        {
            anticlockwise = false;
            startAngle *= -1;
            endAngle *= -1;
        }
        var x = new WebPathFigure()
        {
            StartPoint = from,
            Segments = { new WebArcSegment(point, radius, startAngle, endAngle, anticlockwise, true) }
        };
        BackingObject.Figures.Add(x);
    }

    public void AddLineFigure(Vector prevEnd, Vector nextStart)
    {
        BackingObject.Figures.Add(new()
        {
            StartPoint = prevEnd,
            Segments = { new WebLineSegment(nextStart, true) }
        });
        throw new NotImplementedException();
    }

    public void ClearFigures() => BackingObject.Figures.Clear();
}
