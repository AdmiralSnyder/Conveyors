using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfLib;

namespace WpfApp1;

public class ConveyorPointLane : ICanvasable, ILanePart
{
    public ConveyorPointLane(ConveyorPoint point) => Point = point;
    public Path Arc { get; set; }
    public int Lane { get; internal set; }
    public LinkedListNode<ILanePart> ElementNode { get; internal set; }
    public ConveyorPoint Point { get; }

    public void AddToCanvas(CanvasInfo canvasInfo)
    {
        canvasInfo.Canvas.Children.Add(Arc);
    }

    internal void Prepare()
    {
        if (Point.IsFirst || Point.IsLast) return;
        PrepareArc();
    }

    public void RebuildArc()
    {
        if (Point.IsFirst || Point.IsLast) return;
        var prevEnd = ((ConveyorSegmentLane)ElementNode.Previous.Value).EndPoint;
        var nextStart = ((ConveyorSegmentLane)ElementNode.Next.Value).StartPoint;
        var pg = new PathGeometry()
        { };

        Arc.Data = pg;
        if (Point.LaneStrategy == PointLaneStrategies.StraightLineSegment)
        {
            pg.Figures.Add(new()
            {
                StartPoint = prevEnd,
                Segments = { new LineSegment(nextStart, true) }
            });
        }
        else if (Point.LaneStrategy == PointLaneStrategies.Curve)
        {
            Vector oStart = prevEnd.Subtract(Point.Location);
            var oStartLen = oStart.Length();
            Vector oEnd = nextStart.Subtract(Point.Location);
            var oStartNorm = Divide(oStart, oStartLen);
            var oEndNorm = Normalize(oEnd);
            var dotProd = DotProduct(oStartNorm, oEndNorm);
            var angleRad = Math.Acos(dotProd);

            Arc.Stroke = IsLeft ? Brushes.Plum : Brushes.Tomato;

            bool inside = dotProd > 0.5;

            (bool largeArg, SweepDirection swDir) config = (dotProd > 0.5, IsLeft) switch
            {
                // TODO inside
                (true, true) => (false, SweepDirection.Counterclockwise), // left turn, left side, bad
                (true, false) => (true, SweepDirection.Clockwise),  // right turn, right side, bad
                                                                    // outside
                (false, true) => (false, SweepDirection.Clockwise), // right turn, left side, good
                (false, false) => (false, SweepDirection.Counterclockwise), // left turn, right side, good
            };

            pg.Figures.Add(new()
            {
                StartPoint = prevEnd,
                Segments = { new ArcSegment(nextStart, new(oStartLen, oStartLen), RadToDeg(angleRad), config.largeArg, config.swDir, true) }
            });
        }
    }

    private void PrepareArc()
    {
        Arc = new();
        RebuildArc();
    }

    private bool IsLeft => Lane < Point.Conveyor.LanesCount / 2;

    public double BeginLength { get; set; }

    public double EndLength => BeginLength;

    private static (double x, double y) Divide((double x, double y) vect, double divisor) => Multiply(vect, 1 / divisor);
    private static (double x, double y) Multiply((double x, double y) vect, double factor) => (x: vect.x * factor, y: vect.y * factor);
    private static (double x, double y) Normalize((double x, double y) vect) => Divide(vect, vect.Length());
    private static double DotProduct((double x, double y) a, (double x, double y) b) => a.x * b.x + a.y * b.y;
    private static double RadToDeg(double rad) => Math.PI / rad; 
}