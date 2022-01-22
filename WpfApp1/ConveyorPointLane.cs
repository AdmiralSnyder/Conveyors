using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
        var prevLine = ((ConveyorSegmentLane)ElementNode.Previous.Value).Line;
        var nextLine = ((ConveyorSegmentLane)ElementNode.Next.Value).Line;
        var pg = new PathGeometry()
        { };

        Arc = new()
        {
            //Stroke = Brushes.Plum,
        };
        Arc.Data = pg;
        if (Point.LaneStrategy == PointLaneStrategies.StraightLineSegment)
        {
            pg.Figures.Add(new()
            {
                StartPoint = new(prevLine.X2, prevLine.Y2),
                Segments = { new LineSegment(new(nextLine.X1, nextLine.Y1), true) }
            });
        }
        else if (Point.LaneStrategy == PointLaneStrategies.Curve)
        {
            var oStart = (x: prevLine.X2 - Point.X, y: prevLine.Y2 - Point.Y);
            var oStartLen = oStart.Length();
            var oEnd = (x: nextLine.X2 - Point.X, y: nextLine.Y2 - Point.Y);
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
                StartPoint = new(prevLine.X2, prevLine.Y2),
                Segments = { new ArcSegment(new(nextLine.X1, nextLine.Y1), new(oStartLen, oStartLen), RadToDeg(angleRad), config.largeArg, config.swDir, true) }
            });
        }
    }

    private bool IsLeft => Lane < Point.Conveyor.LanesCount / 2;

    private static (double x, double y) Divide((double x, double y) vect, double divisor) => Multiply(vect, 1 / divisor);
    private static (double x, double y) Multiply((double x, double y) vect, double factor) => (x: vect.x * factor, y: vect.y * factor);
    private static (double x, double y) Normalize((double x, double y) vect) => Divide(vect, vect.Length());
    private static double DotProduct((double x, double y) a, (double x, double y) b) => a.x * b.x + a.y * b.y;
    private static double RadToDeg(double rad) => Math.PI / rad; 
}