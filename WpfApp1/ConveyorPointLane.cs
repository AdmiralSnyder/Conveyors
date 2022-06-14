using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfLib;

namespace WpfApp1;

public class ConveyorPointLane : ICanvasable, ILanePart, ISelectObject, IRefreshable
{
    public ConveyorPointLane(ConveyorPoint point) => Point = point;
    private bool IsLeft => Lane < Point.Conveyor.LanesCount / 2;
    public double BeginLength { get; set; }
    public double Length { get; private set; }
    public double EndLength => BeginLength + Length;
    public string Text => $"PointLane {Point.Conveyor.Number}.{Point.Number}.{Lane} ({Length :2})";
    public Path? Arc { get; private set; }
    public int Lane { get; internal set; }

    public bool Inside { get; private set; }
    public Angle AngleRadPoint => Point.Angle;

    public LinkedListNode<ILanePart> ElementsNode { get; internal set; }
    public LinkedListNode<ConveyorPointLane> Node { get; internal set; }
    public ConveyorPoint Point { get; }

    public void AddToCanvas(CanvasInfo canvasInfo)
    {
        Arc = canvasInfo.ShapeProvider.CreateConveyorPointPath(ArcGeometry, IsLeft);
        canvasInfo.Canvas.Children.Add(Arc);
        Arc!.Tag = this;
    }

    internal void Prepare()
    {
        if (Point.IsFirst || Point.IsLast) return;
        RebuildArc();
    }

    private TwoPoints ArcStartEnd { get; set; }
    private PathGeometry ArcGeometry { get; set; } = new();
    private double ArcAngleRad { get; set; }

    public Point[] SelectionBoundsPoints { get; } = new Point[2];

    public ISelectObject? SelectionParent => Point;

    public void RebuildArc()
    {
        if (Point.IsFirst || Point.IsLast) return;

        var prevEnd =  ((ConveyorSegmentLane)ElementsNode.Previous.Value).EndPoint;
        var nextStart = ((ConveyorSegmentLane)ElementsNode.Next.Value).StartPoint;
        
        ArcStartEnd = (prevEnd, nextStart);

        var selectionBoundsPoints = (Point[])SelectionBoundsPoints;
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
            Vector oStart = Point.Location.Subtract(prevEnd);
            var oStartLen = oStart.Length();
            //Vector oEnd = nextStart.Subtract(Point.Location);
            //var oStartNorm = oStart.Normalize(oStartLen);
            //var oEndNorm = oEnd.Normalize();
            //var dotProd = oStartNorm.DotProduct(oEndNorm);
            var angle = Point.Angle;
            

            bool toTheLeft = angle.IsAcute;

            // TODO correctly calculate the inside property.
            Inside = toTheLeft ? IsLeft : !IsLeft;

            var (largeArg, swDir) = (!Point.IsClockwise, IsLeft) switch
            {
                // TODO inside
                (true, true) => (false, SweepDirection.Counterclockwise), // left turn, left side, bad
                (true, false) => (true, SweepDirection.Clockwise),  // right turn, right side, bad
                                                                    // outside
                (false, true) => (false, SweepDirection.Clockwise), // right turn, left side, good
                (false, false) => (false, SweepDirection.Counterclockwise), // left turn, right side, good
            };

            ArcGeometry.Figures.Add(new()
            {
                StartPoint = prevEnd,
                Segments = { new ArcSegment(nextStart, new(oStartLen, oStartLen), angle.Degrees, largeArg, swDir, true) }
            });

            Length = /*2 * Math.PI * */oStartLen * angle.Radians/* / 2 * Math.PI*/;
        }
    }

    double LastLength = 0;
    public Point GetPointAbsolute(double length, bool overshoot = false)
    {
        LastLength = length;
        if (Point.LaneStrategy == PointLaneStrategies.Curve)
        {
            var relLen = (length - BeginLength) / Length;
            var angleFactor = (overshoot ? relLen : Math.Min(1.0, relLen));
            if (Inside)
            {
                if (Point.Angle.IsClockwise)
                {
                    return ArcStartEnd.P1.RotateAround(Point.Location, - Point.Angle.Radians * angleFactor);
                }
                else
                {
                    var rotPoint = ArcStartEnd.P2.Add(Point.Location.To(ArcStartEnd.P1));
                    return ArcStartEnd.P1.RotateAround(rotPoint, Point.Angle.Radians * angleFactor);
                }
            }
            else
            {
                var rotPoint = ArcStartEnd.P2.Add(Point.Location.To(ArcStartEnd.P1));
                return ArcStartEnd.P1.RotateAround(rotPoint, -Point.Angle.Radians * angleFactor);
            }
        }
        else
        {
            // TODO precalculate stuff
            return ArcStartEnd.GetPointOnLine(length - BeginLength, overshoot);
        }
    }
}