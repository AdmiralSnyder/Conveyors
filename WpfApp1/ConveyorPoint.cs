using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfLib;
namespace WpfApp1;

public interface IElementsNode<T>
{
    public LinkedListNode<IPathPart> ElementsNode { get; }
}

public interface IListNode<T>
{
    public LinkedListNode<T> Node { get; }
}

public class ConveyorPoint : ICanvasable, IPathPart, ISelectObject, IElementsNode<IPathPart>, IListNode<ConveyorPoint>, IRefreshable
{
    public string Text => $"Point {Conveyor.Number}.{Number} ({Location})";

    public ConveyorPoint(Conveyor conveyor)
    {
        Conveyor = conveyor;
        Lanes = new ConveyorPointLane[conveyor.LanesCount];
        LaneStrategy = PointLaneStrategies.Curve;
        Number = Conveyor.Points.Count;
    }

    public bool IsLast { get; internal set; }
    public bool IsFirst { get; internal set; }

    private Point _Location;
    public Point Location
    {
        get => _Location;
        set => Func.Setter(ref _Location, value, () =>
        {
            if (PointCircle is not null)
            {
                PointCircle.SetCenterLocation(Location);
            }
            var (prev, next) = GetAdjacentSegments();
            if (prev is { })
            {
                prev.StartEnd = (prev.StartEnd.P1, Location);
                prev.GetAdjacentPoints().prev?.PreparePoint();
            }

            ((ISelectObject)this).SetSelectionPoints(_Location);

            PreparePoint();

            if (next is { })
            {
                next.StartEnd = (Location, next.StartEnd.P2);
                next.GetAdjacentPoints().next?.PreparePoint();
            }


            (prev?.GetAdjacentPoints().prev ?? this)?.RebuildLanes();

            //RebuildLanes();

            //next?.GetAdjacentPoints().next?.RebuildLanes();

            // TODO this might better be a method of the lanes?

            //double len = 0d;
            //foreach (var i in Conveyor.LaneIndexes)
            //{
            //    if (prev is { })
            //    {
            //        len = prev.Lanes[i]?.EndLength ?? 0d;
            //    }
            //    var elNode = next?.Lanes[i]?.ElementsNode;
            //    while (elNode is { })
            //    {
            //        var element = elNode.Value;
            //        element.BeginLength = len;
            //        len = element.EndLength;
            //        elNode = elNode.Next;
            //    }
            //}

            // This is utterly dirty - the conveyor should be a listener on the point's locations - or rather, all locations...
            ((ISelectObject)Conveyor).SetSelectionPoints();
        });
    }

    public void RebuildLanes()
    {
        foreach (var lane in Lanes)
        {
            if (lane is not null)
            {
                lane.RebuildArc();
                if (lane.ElementsNode.Previous?.Value is { } prev)
                {
                    lane.BeginLength = prev.EndLength;
                }
            }
        }

        this.ElementsNode.Next?.Value?.RebuildLanes();
    }

    public ConveyorPointLane[] Lanes;

    public PointLaneStrategies LaneStrategy { get; }
    public int Number { get; }
    public Ellipse PointCircle { get; internal set; }
    public Conveyor Conveyor { get; }
    public LinkedListNode<ConveyorPoint> Node { get; internal set; }
    public LinkedListNode<IPathPart> ElementsNode { get; internal set; }

    public Vector Incoming { get; private set; }
    public Vector IncomingNorm { get; private set; }
    public Vector IncomingNormInversed { get; private set; }

    public Vector Outgoing { get; private set; }
    public Vector OutgoingNorm { get; private set; }

    public Angle Angle { get; private set; }
    
    public Angle AbsoluteAngle { get; private set; }

    public Angle IncomingAngle { get; private set; }
    public Angle OutgoingAngle { get; private set; }

    public bool IsClockwise { get; private set; }
    public bool IsStraight { get; private set; }

    public Point[] SelectionBoundsPoints { get; } = new Point[1];

    public ISelectObject? SelectionParent => Conveyor;

    public void AddToCanvas(CanvasInfo canvasInfo)
    {
        PointCircle = canvasInfo.ShapeProvider.CreateConveyorPointEllipse(Location, IsFirst, IsLast, IsClockwise, IsStraight);
        PointCircle.Tag = this;
        canvasInfo.Canvas.Children.Add(PointCircle);

        if (IsFirst || IsLast) return;
        foreach (var lane in Lanes)
        {
            lane.AddToCanvas(canvasInfo);
        }
    }

    internal void CreateLanes()
    {
        if (IsFirst || IsLast) return;

        foreach (var i in Conveyor.LaneIndexes)
        {
            var lane = Lanes[i] = new(this);
            lane.Lane = i;
        }
    }

    internal void PreparePoint()
    {
        var prevNode = Node.Previous;
        var nextNode = Node.Next;

        if (prevNode is not null && nextNode is not null)
        {

            var prevLocation = ((ConveyorPoint)prevNode.Value).Location;
            var nextLocation = ((ConveyorPoint)nextNode.Value).Location;

            Incoming = Location.Subtract(prevLocation);
            IncomingNorm = Incoming.Normalize();
            IncomingNormInversed = IncomingNorm.Inverse();

            Outgoing = nextLocation.Subtract(Location);
            OutgoingNorm = Outgoing.Normalize();

            var dotInX = IncomingNorm.DotProduct(Maths.XAxisV1);
            var dotOutX = OutgoingNorm.DotProduct(Maths.XAxisV1);

            IncomingAngle = Math.Acos(dotInX).Radians();
            OutgoingAngle = Math.Acos(dotOutX).Radians();

            Angle = Maths.AngleBetween(IncomingNormInversed, OutgoingNorm);
            AbsoluteAngle = Math.Abs(Angle.Radians).Radians();

            IsClockwise = Angle.Radians < 0;
            IsStraight = Angle.IsStraight;
        }

        ((IRefreshable)this).NotifyRefresh();
    }

    public void RegisterLanes()
    {
        if (IsFirst || IsLast) return;

        foreach (var lane in Lanes)
        {
            lane.ElementsNode = Conveyor.PointAndSegmentLanes[lane.Lane].AddLast(lane);
            lane.Node = Conveyor.PointLanes[lane.Lane].AddLast(lane);
        }
    }

    internal void PrepareLanes()
    {
        if (IsFirst || IsLast) return;

        foreach (var lane in Lanes)
        {
            lane.Prepare();
        }
    }

    internal (ConveyorSegment? prev, ConveyorSegment? next) GetAdjacentSegments() => (
        ElementsNode.Previous?.Value as ConveyorSegment,
        ElementsNode.Next?.Value as ConveyorSegment
    );
}

public enum PointLaneStrategies
{
    StraightLineSegment,
    Curve,
}