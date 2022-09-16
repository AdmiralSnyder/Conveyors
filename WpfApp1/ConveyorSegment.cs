using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfLib;

namespace WpfApp1;

public class ConveyorSegment : ICanvasable, IPathPart, ISelectObject, IRefreshable
{
    public string Text => $"Segment {Conveyor.Number}.{Number} ({StartEnd})";

    public ConveyorSegment(Conveyor conv, double length, TwoPoints startEnd)
    {
        Conveyor = conv;
        Lanes = new ConveyorSegmentLane[conv.LanesCount];
        DefinitionLength = length;
        Number = conv.Segments.Count;
        StartEnd = startEnd;
    }

    public Line? DefinitionLine { get; set; }
    private TwoPoints _StartEnd;
    public TwoPoints StartEnd 
    {
        get => _StartEnd;
        set => Func.Setter(ref _StartEnd, value, () =>
        {
            if (DefinitionLine is { })
            {
                DefinitionLine.SetLocation(StartEnd);
            }

            foreach (var i in Conveyor.LaneIndexes)
            {
                Lanes[i]?.Rebuild();
            }
            ((ISelectObject)this).SetSelectionPoints(StartEnd.P1, StartEnd.P2);
        });
    }

    internal (ConveyorPoint? prev, ConveyorPoint? next) GetAdjacentPoints() => (
        ElementsNode.Previous?.Value as ConveyorPoint,
        ElementsNode.Next?.Value as ConveyorPoint
    );

    internal ConveyorPoint? GetPreviousPoint() => ElementsNode.Next?.Value as ConveyorPoint;
    internal ConveyorPoint? GetNextPoint() => ElementsNode.Next?.Value as ConveyorPoint;

    internal ConveyorPoint? TryGetPreviousPoint(int desiredLevel = 0)
    {
        if (desiredLevel < 0) return null;

        var segmentNode = ElementsNode;
        ConveyorPoint? result = segmentNode?.Previous?.Value as ConveyorPoint;
        while (desiredLevel-- > 0)
        {
            segmentNode = segmentNode?.Previous?.Previous;
            if (segmentNode?.Previous?.Value is ConveyorPoint cp)
            {
                result = cp;
            }
        }
       
        return segmentNode?.Previous?.Value as ConveyorPoint ?? result;
    }

    internal ConveyorPoint? TryGetNextPoint(int desiredLevel = 0)
    {
        if (desiredLevel < 0) return null;

        var segmentNode = ElementsNode;
        ConveyorPoint? result = segmentNode?.Next?.Value as ConveyorPoint;
        while (desiredLevel-- > 0)
        {
            segmentNode = segmentNode?.Next?.Next;
            if (segmentNode?.Next?.Value is ConveyorPoint cp)
            {
                result = cp;
            }
        }

        return segmentNode?.Next?.Value as ConveyorPoint ?? result;
    }

    public ConveyorSegmentLane[] Lanes { get; }
    public int Number { get; }
    public Conveyor Conveyor { get; }

    public double DefinitionLength { get; set; }
    public LinkedListNode<ConveyorSegment> Node { get; internal set; }
    public LinkedListNode<IPathPart> ElementsNode { get; internal set; }

    public Point[] SelectionBoundsPoints { get; } = new Point[2];

    public ISelectObject? SelectionParent => Conveyor;

    public void AddToCanvas(CanvasInfo canvasInfo)
    {
        DefinitionLine = canvasInfo.ShapeProvider.CreateConveyorSegmentLine(StartEnd);
        DefinitionLine.Tag = this;

        canvasInfo.Canvas.Children.Add(DefinitionLine);
        foreach(var lane in Lanes)
        {
            lane.AddToCanvas(canvasInfo);
        }
    }

    internal void CreateLanes()
    {
        foreach (int i in Conveyor.LaneIndexes)
        {
            var laneList = Conveyor.SegmentLanes[i];
            var prevSegment = laneList.Last;
            ConveyorSegmentLane lane = new(i, this);
            Lanes[i] = lane;
            lane.Node = laneList.AddLast(lane);
        }
    }
    
    public const int LineDistance = 10;

    public void RegisterLanes()
    {
        foreach (var lane in Lanes)
        {
            lane.ElementsNode = Conveyor.PointAndSegmentLanes[lane.LaneNumber].AddLast(lane);
        }
    }

    public void RebuildLanes()
    {
        ElementsNode.Next?.Value?.RebuildLanes();
    }

    public void UpdateLengths()
    {
        foreach (var lane in Lanes)
        {
            if (lane?.ElementsNode.Previous?.Value is { } prev)
            {
                lane.BeginLength = prev.EndLength;
            }
        }

        ElementsNode.Next?.Value?.UpdateLengths();
    }
}
