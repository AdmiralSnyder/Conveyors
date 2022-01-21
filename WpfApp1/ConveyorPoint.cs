﻿using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1;

public class ConveyorPoint : ICanvasable, IPathPart
{
    public ConveyorPoint(Conveyor conveyor)
    {
        Conveyor = conveyor;
        Lanes = new ConveyorPointLane[conveyor.LanesCount];
        LaneStrategy = PointLaneStrategies.Curve;
    }

    public bool IsLast { get; internal set; }
    public bool IsFirst { get; internal set; }
    public double Y { get; internal set; }
    public double X { get; internal set; }

    public ConveyorPointLane[] Lanes;

    public PointLaneStrategies LaneStrategy { get; }
    public Ellipse PointCircle { get; internal set; }
    public Conveyor Conveyor { get; }
    public LinkedListNode<ConveyorPoint> Node { get; internal set; }
    public LinkedListNode<IPathPart> ElementsNode { get; internal set; }

    private const double Size = 4d;

    public void AddToCanvas(Canvas canvas)
    {
        PointCircle = new() { Width = Size, Height = Size, Fill = IsLast ? Brushes.Red : IsFirst ? Brushes.Cyan : Brushes.Blue };
        canvas.Children.Add(PointCircle);
        Canvas.SetLeft(PointCircle, X - Size / 2.0);
        Canvas.SetTop(PointCircle, Y - Size / 2.0);

        if (IsFirst || IsLast) return;
        foreach (var lane in Lanes)
        {
            lane.AddToCanvas(canvas);
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
}

public enum PointLaneStrategies
{
    StraightLineSegment,
    Curve,
}