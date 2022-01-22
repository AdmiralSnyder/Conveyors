using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
    }

    public Line? DefinitionLine { get; set; }
    public TwoPoints StartEnd { get; set; }
    public ConveyorSegmentLane[] Lanes { get; }
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
            var line = CreateLine(StartEnd, i);
            ConveyorSegmentLane lane = new(prevSegment?.Value.EndLength ?? 0, line);
            Lanes[i] = lane;
            lane.Lane = i;
            lane.Node = laneList.AddLast(lane);
        }
    }
    

    private Line CreateLine(TwoPoints original, int idx)
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

        return OffsettedLine(original, scaledOffset);    
    }

    const int LineDistance = 10;

    private (double x, double y) Normalize((double x, double y) vect)
    {
        var len = Length(vect);
        return (vect.x * 1 / len, vect.y * 1 / len);
    }

    private double Length((double x, double y) vect) => Math.Sqrt(vect.x * vect.x + vect.y * vect.y);

    private Line OffsettedLine(TwoPoints original, double offset)
    {
        var origVect = (original.P2.X - original.P1.X, original.P2.Y - original.P1.Y);
        var origNormalVect = Normalize(origVect);
        var offsetNormalVect = (x: -origNormalVect.y, y: origNormalVect.x);
        var offsetVect = (x: offsetNormalVect.x * offset, y: offsetNormalVect.y * offset);

        return new Line()
        {
            X1 = original.P1.X + offsetVect.x,
            X2 = original.P2.X + offsetVect.x,
            Y1 = original.P1.Y + offsetVect.y,
            Y2 = original.P2.Y + offsetVect.y,
            Stroke = Brushes.White,
        };
    }

    public void RegisterLanes()
    {
        foreach (var lane in Lanes)
        {
            lane.ElementNode = Conveyor.PointAndSegmentLanes[lane.Lane].AddLast(lane);
        }
    }
}
