using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1;

public interface ICanvasable
{
    void AddToCanvas(Canvas canvas);
}

public class ConveyorSegment : ICanvasable, IPathPart
{
    public ConveyorSegment(Conveyor conv, double length, Line line)
    {
        DefinitionLength = length;
        DefinitionLine = line;
        Conveyor = conv;
        Lanes = new ConveyorSegmentLane[conv.LanesCount];
    }

    public Line DefinitionLine { get; set; }
    public ConveyorSegmentLane[] Lanes { get; }
    public Conveyor Conveyor { get; }

    public double DefinitionLength { get; set; }
    public LinkedListNode<ConveyorSegment> Node { get; internal set; }
    public LinkedListNode<IPathPart> ElementsNode { get; internal set; }

    public void AddToCanvas(Canvas canvas)
    {
        canvas.Children.Add(DefinitionLine);
        foreach(var lane in Lanes)
        {
            lane.AddToCanvas(canvas);
        }
    }

    internal void BuildLanes()
    {
        foreach (int i in Conveyor.LaneIndexes)
        {
            var laneList = Conveyor.SegmentLanes[i];
            var prevSegment = laneList.Last;
            var line = CreateLine(DefinitionLine, i);
            ConveyorSegmentLane lane = new(prevSegment?.Value.EndLength ?? 0, line);
            Lanes[i] = lane;
            lane.Lane = i;
            lane.Node = laneList.AddLast(lane);
        }
    }
    

    private Line CreateLine(Line original, int idx)
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

    private Line OffsettedLine(Line original, double offset)
    {
        var origVect = (original.X2 - original.X1, original.Y2 - original.Y1);
        var origNormalVect = Normalize(origVect);
        var offsetNormalVect = (x: -origNormalVect.y, y: origNormalVect.x);
        var offsetVect = (x: offsetNormalVect.x * offset, y: offsetNormalVect.y * offset);

        return new Line()
        {
            X1 = original.X1 + offsetVect.x,
            X2 = original.X2 + offsetVect.x,
            Y1 = original.Y1 + offsetVect.y,
            Y2 = original.Y2 + offsetVect.y,
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
