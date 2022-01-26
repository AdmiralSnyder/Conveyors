using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfLib;
namespace WpfApp1;

public class ConveyorPoint : ICanvasable, IPathPart, ISelectObject
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
    //public double Y { get; internal set; }
    //public double X { get; internal set; }
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
                prev.GetAdjacentPoints().prev?.RebuildLanes();
            }

            if (next is { })
            {
                next.StartEnd = (Location, next.StartEnd.P2);
                next.GetAdjacentPoints().next?.RebuildLanes();
            }

            RebuildLanes();

            // TODO this might better be a method of the lanes?
            double len = 0d;
            foreach (var i in Conveyor.LaneIndexes)
            {
                if (prev is { })
                {
                    len = prev.Lanes[i]?.EndLength ?? 0d;
                }
                var elNode = next?.Lanes[i]?.ElementNode;
                while(elNode is { })
                {
                    var element = elNode.Value;
                    element.BeginLength = len;
                    len = element.EndLength;
                    elNode = elNode.Next;
                }
            }
        });
    }

    public void RebuildLanes()
    {
        foreach (var lane in Lanes)
        {
            lane?.RebuildArc();
        }
    }

    public ConveyorPointLane[] Lanes;

    public PointLaneStrategies LaneStrategy { get; }
    public int Number { get; }
    public Ellipse PointCircle { get; internal set; }
    public Conveyor Conveyor { get; }
    public LinkedListNode<ConveyorPoint> Node { get; internal set; }
    public LinkedListNode<IPathPart> ElementsNode { get; internal set; }

    public void AddToCanvas(CanvasInfo canvasInfo)
    {
        PointCircle = canvasInfo.ShapeProvider.CreateConveyorPointEllipse(Location, IsFirst, IsLast);
        PointCircle.Tag = this;
        canvasInfo.Canvas.Children.Add(PointCircle);

        if (IsFirst || IsLast) return;
        foreach (var lane in Lanes)
        {
            lane.AddToCanvas(canvasInfo);
        }
    }

    internal void BuildLanes()
    {
        if (IsFirst || IsLast) return;

        foreach (var i in Conveyor.LaneIndexes)
        {
            var lane = Lanes[i] = new(this);
            lane.Lane = i;
        }
    }

    public void RegisterLanes()
    {
        if (IsFirst || IsLast) return;

        foreach (var lane in Lanes)
        {
            lane.ElementNode = Conveyor.PointAndSegmentLanes[lane.Lane].AddLast(lane);
            lane.Node = Conveyor.PointLanes[lane.Lane].AddLast(lane);
            if (lane.ElementNode.Previous?.Value is { } prev)
            {
                lane.BeginLength = prev.EndLength;
            }
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