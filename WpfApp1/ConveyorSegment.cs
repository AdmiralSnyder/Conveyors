using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfLib;

namespace WpfApp1;

public class CanvasInfo
{
    public Canvas Canvas { get; set; }
    public ConveyorShapeProvider ShapeProvider { get; set; }
}

public interface ICanvasable
{
    void AddToCanvas(CanvasInfo canvasInfo);
}

public class ConveyorSegment : ICanvasable, IPathPart
{
    public ConveyorSegment(Conveyor conv, double length, TwoPoints startEnd)
    {
        DefinitionLength = length;
        StartEnd = startEnd;
        Conveyor = conv;
        Lanes = new ConveyorSegmentLane[conv.LanesCount];
        Number = conv.Segments.Count;
    }

    public Line? DefinitionLine { get; set; }
    public TwoPoints StartEnd { get; set; }
    public ConveyorSegmentLane[] Lanes { get; }
    public int Number { get; }
    public Conveyor Conveyor { get; }

    public double DefinitionLength { get; set; }
    public LinkedListNode<ConveyorSegment> Node { get; internal set; }
    public LinkedListNode<IPathPart> ElementsNode { get; internal set; }

    public void AddToCanvas(CanvasInfo canvasInfo)
    {
        DefinitionLine = canvasInfo.ShapeProvider.CreateConveyorSegmentLine(StartEnd);
        canvasInfo.Canvas.Children.Add(DefinitionLine);
        foreach(var lane in Lanes)
        {
            lane.AddToCanvas(canvasInfo);
        }
    }

    internal void BuildLanes()
    {
        foreach (int i in Conveyor.LaneIndexes)
        {
            var laneList = Conveyor.SegmentLanes[i];
            var prevSegment = laneList.Last;
            var line = GetLanePoints(StartEnd, i);
            ConveyorSegmentLane lane = new(prevSegment?.Value.EndLength ?? 0, line, i, this);
            Lanes[i] = lane;
            lane.Node = laneList.AddLast(lane);
        }
    }
    

    private TwoPoints GetLanePoints(TwoPoints original, int idx)
    {
            //1    0
            //2   -0.5  0.5
            //3   -1  0  1
            //4   -1.5 -0.5  0.6  1.5
        var laneCount = Conveyor.LanesCount;
        var steps = laneCount - 1;
        var leftmost = -steps / 2.0;
        var offset = leftmost + idx;
        var scaledOffset = offset * LineDistance;

        return OffsetLanePoints(original, scaledOffset);    
    }

    const int LineDistance = 10;

    private TwoPoints OffsetLanePoints(TwoPoints original, double offset)
    {
        var origVect = original.Vector();
        var origNormalVect = origVect.Normalize();
        Vector offsetNormalVect = (-origNormalVect.Y, origNormalVect.X);
        var offsetVect = offsetNormalVect.Multiply(offset);
        return original.Add(offsetVect);
    }

    public void RegisterLanes()
    {
        foreach (var lane in Lanes)
        {
            lane.ElementNode = Conveyor.PointAndSegmentLanes[lane.Lane].AddLast(lane);
        }
    }
}
