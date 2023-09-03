using System.Diagnostics;
using System.Windows.Media;
using UILib.Shapes;

namespace ConveyorLib;

[DebuggerDisplay($"{"PointLane"} ({{{nameof(ConveyorPointLane.DebugText)}}})")]
public class ConveyorPointLane : IConveyorCanvasable, ILanePart, IDebugText, ISelectObject, IRefreshable
{
    public ConveyorPointLane(ConveyorPoint point)
    {
        Point = point;
        Number = point.Conveyor.PointLanes[0].Count;
    }

    private bool IsLeft => Lane < Point.Conveyor.LanesCount / 2;
    public double BeginLength { get; set; }
    public double Length { get; private set; }
    public double EndLength => BeginLength + Length;

    public string DebugText => $"{Point.Conveyor.Number}.{Point.Number}.{Lane} ({Length:2})";
    public string Text => $"PointLane {DebugText}";
    public IPath? Arc { get; private set; }
    public int Lane { get; internal set; }

    public bool Inside { get; private set; }
    public Angle AngleRadPoint => Point.Angle;

    public LinkedListNode<ILanePart> ElementsNode { get; internal set; }
    public LinkedListNode<ConveyorPointLane> Node { get; internal set; }
    public ConveyorPoint Point { get; }
    public int Number { get; }

    public void AddToCanvas(IConveyorCanvasInfo canvasInfo)
    {
        Arc = canvasInfo.ShapeProvider.CreateConveyorPointPath(ArcGeometry, IsLeft);
        canvasInfo.AddToCanvas(Arc);
        Arc!.Tag = this;
    }

    internal void Prepare()
    {
        if (Point.IsFirst || Point.IsLast) return;
        RebuildArc();
    }

    private TwoPoints ArcStartEnd { get; set; }
    private PathGeometry ArcGeometry { get; set; } = new();

    private Point[] SelectionBoundsPoints = new Point[2];
    public Point[] GetSelectionBoundsPoints() => SelectionBoundsPoints;


    public ISelectObject? SelectionParent => Point;

    // $.AddConveyor(new V2d[]{(170, 160), (180, 160), (180, 150), (190, 150), (190, 160), (200, 160)}, false, 2); 
    public void RebuildArc()
    {
        if (Point.IsFirst || Point.IsLast) return;

        var prevEnd = ((ConveyorSegmentLane)ElementsNode.Previous.Value).EndPoint;
        var nextStart = ((ConveyorSegmentLane)ElementsNode.Next.Value).StartPoint;

        ArcStartEnd = (prevEnd, nextStart);

        ((ISelectObject)this).SetSelectionPoints(prevEnd, nextStart);
        // TODO add 3rd

        ArcGeometry.Figures.Clear();

        if (Point.LaneStrategy == PointLaneStrategies.StraightLineSegment)
        {
            ArcGeometry.Figures.Add(new()
            {
                StartPoint = prevEnd,
                Segments = { new LineSegment(nextStart, true) }
            });
            Length = new TwoPoints(prevEnd, nextStart).Length();
        }
        else if (Point.LaneStrategy == PointLaneStrategies.Curve)
        {
            Vector oStart = new(prevEnd, Point.Location);
            var radius = oStart.Length();

            //var radius = ConveyorSegment.LineDistance / 2;

            //Vector oEnd = nextStart.Subtract(Point.Location);
            //var oStartNorm = oStart.Normalize(oStartLen);
            //var oEndNorm = oEnd.Normalize();
            //var dotProd = oStartNorm.DotProduct(oEndNorm);

            bool clockwise = !Point.IsClockwise;

            // TODO correctly calculate the inside property.
            Inside = IsLeft == clockwise;

            var (largeArc, swDir) = (clockwise, IsLeft) switch
            {
                // TODO inside
                (true, true) => (false, SweepDirection.Counterclockwise), // left turn, left side, bad
                (true, false) => (false, SweepDirection.Counterclockwise),  // right turn, right side, bad
                                                                            // outside
                (false, true) => (false, SweepDirection.Clockwise), // right turn, left side, good
                (false, false) => (false, SweepDirection.Clockwise), // left turn, right side, good
            };

            if (Inside)
            {
                var previousSegmentLane = (ConveyorSegmentLane)ElementsNode.Previous.Value;
                var P1 = previousSegmentLane.StartPoint;
                var P2 = previousSegmentLane.EndPoint;

                var nextSegmentLane = (ConveyorSegmentLane)ElementsNode.Next.Value;
                var R1 = nextSegmentLane.StartPoint;
                var R2 = nextSegmentLane.EndPoint;

                //if (previousSegmentLane.Length == 0 || nextSegmentLane.Length == 0 || Maths.VectorsAreParallel(new(P1, P2), new(R1, R2)) || Maths.VectorsAreInverseParallel(new(P1, P2), new(R1, R2)))
                //{
                //    if (ElementsNode.Previous?.Value is ConveyorSegmentLane prevSegLane)
                //    {
                //        prevSegLane.EndPoint = P2;
                //    }

                //    if (ElementsNode.Next?.Value is ConveyorSegmentLane nextSegLane)
                //    {
                //        nextSegLane.StartPoint = R1;
                //    }
                //}
                //else
                //{
                var yr1 = R1.Y;
                var xp = P2.X - P1.X;
                var yp1 = P1.Y;
                var xr1 = R1.X;
                var yp = P2.Y - P1.Y;
                var xp1 = P1.X;
                var xr = R2.X - R1.X;
                var yr = R2.Y - R1.Y;
                var quotient = (xr * yp - yr * xp);
                if (quotient == 0)
                {
                    if (ElementsNode.Previous?.Value is ConveyorSegmentLane prevSegLane)
                    {
                        prevSegLane.EndPoint = P2;
                    }

                    if (ElementsNode.Next?.Value is ConveyorSegmentLane nextSegLane)
                    {
                        nextSegLane.StartPoint = R1;
                    }
                }
                else
                {
                    var sr = (yr1 * xp - yp1 * xp - xr1 * yp + xp1 * yp) / quotient; // TODO what happens if zero??

                    var xq = xr1 + sr * (R2.X - R1.X);
                    var yq = yr1 + sr * (R2.Y - R1.Y);

                    var cross = new Vector(xq, yq);
                    var start = R1;
                    var end = P2;
                    var CrossStart = start - cross;
                    var CrossEnd = end - cross;
                    bool x = true;
                    Point ActStart;
                    if (x)
                    {
                        ActStart = (start - CrossStart) - CrossEnd;
                    }
                    else
                    {
                        ActStart = CrossEnd;
                    }
                    var ActEnd = (end - CrossStart) - CrossEnd;

                    ArcGeometry.Figures.Add(new()
                    {
                        StartPoint = ActStart,
                        Segments = { new ArcSegment(ActEnd, new(radius, radius), Point.Angle.Degrees, largeArc, swDir, true) }
                    });

                    if (ElementsNode.Previous?.Value is ConveyorSegmentLane prevSegLane)
                    {
                        prevSegLane.EndPoint = ActStart;
                    }

                    if (ElementsNode.Next?.Value is ConveyorSegmentLane nextSegLane)
                    {
                        nextSegLane.StartPoint = ActEnd;
                    }
                }
            }
            else
            {
                ArcGeometry.Figures.Add(new()
                {
                    StartPoint = prevEnd,
                    Segments = { new ArcSegment(nextStart, new(radius, radius), Point.Angle.Degrees, largeArc, swDir, true) }
                });
            }



            Length = (Angle.HalfCircle - Point.AbsoluteAngle).Radians * radius;
        }
    }

    double LastLength = 0;
    public Point GetPointAbsolute(double length, bool overshoot = false)
    {
        LastLength = length;
        if (Point.LaneStrategy == PointLaneStrategies.StraightLineSegment)
        {
            // TODO precalculate stuff
            return ArcStartEnd.GetPointOnLine(length - BeginLength, overshoot);
        }
        else
        {
            var relLen = (length - BeginLength) / Length;
            var intepolationFactor = (overshoot ? relLen : Math.Min(1.0, relLen));
            if (Inside)
            {
                var rotPoint = ArcStartEnd.P2.Add(Point.Location.To(ArcStartEnd.P1));
                return ArcStartEnd.P1.RotateAround(rotPoint, Point.Angle.CounterAngle() * intepolationFactor);
            }
            else
            {
                return ArcStartEnd.P1.RotateAround(Point.Location, ~Point.Angle.CounterAngle() * intepolationFactor);
            }
        }
    }
}