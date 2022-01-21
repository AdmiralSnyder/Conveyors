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

    public void AddToCanvas(Canvas canvas)
    {
        canvas.Children.Add(Arc);
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
            Stroke = Brushes.Plum,
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
            var len = oStart.Length();
            pg.Figures.Add(new()
            {
                StartPoint = new(prevLine.X2, prevLine.Y2),
                Segments = { new ArcSegment(new(nextLine.X1, nextLine.Y1), new(len, len), 45, false, SweepDirection.Counterclockwise, true) }
            });
        }

    }
}