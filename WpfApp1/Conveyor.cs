using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1;

public class Conveyor
{
    private bool _IsRunning;
    public bool IsRunning
    {
        get => _IsRunning;
        set => Func.Setter(ref _IsRunning, value, StartIfRunning);
    }

    public int LanesCount { get; init; } = 1;
    public IEnumerable<int> LaneIndexes;

    private void StartIfRunning()
    {
        if (IsRunning)
        {
            (Dispatcher = new(new ParameterizedThreadStart(ItemDispatcherThreadAction))).Start(this);
        }
    }

    public LinkedList<ConveyorSegment> Segments = new();
    public LinkedList<ConveyorSegmentLane>[] SegmentLanes;

    public LinkedList<ConveyorPoint> Points = new();
    public LinkedList<IPathPart> PointsAndSegments = new();
    public LinkedList<ConveyorPointLane>[] PointLanes;
    public LinkedList<ILanePart>[] PointAndSegmentLanes;
    public static Conveyor Create(IEnumerable<Line> lines, int lanesCount = 1)
    {
        var conv = new Conveyor(lanesCount);
        double length = 0d;
        bool first = true;
        {
            ConveyorPoint? point = null;
            foreach (var line in lines)
            {
                if (first)
                {
                    first = false;
                    point = AddPoint(line.X1, line.Y1);
                    point.IsFirst = true;
                }
                var segment = new ConveyorSegment(conv, length, line);
                segment.Node = conv.Segments.AddLast(segment);
                segment.ElementsNode = conv.PointsAndSegments.AddLast(segment);

                length += segment.DefinitionLength;

                point = AddPoint(line.X2, line.Y2);
            }
            if (point is not null)
            {
                point.IsLast = true;
            }
        }
        foreach (var segment in conv.Segments)
        {
            segment.BuildLanes();
        }

        foreach (var point in conv.Points)
        {
            point.BuildLanes();
        }

        foreach (var part in conv.PointsAndSegments)
        {
            part.RegisterLanes();
        }

        foreach (var point in conv.Points)
        {
            point.PrepareLanes();
        }

        return conv;

        ConveyorPoint AddPoint(double x, double y)
        {
            ConveyorPoint point = new(conv) { X = x, Y = y };
            point.Node = conv.Points.AddLast(point);
            point.ElementsNode = conv.PointsAndSegments.AddLast(point);
            return point;
        }
    }

    public static void AddToCanvas(Conveyor conveyor, Canvas canvas)
    {
        conveyor.Canvas = canvas;
        foreach (var segment in conveyor.Segments)
        {
            segment.AddToCanvas(canvas);
        }

        foreach (var point in conveyor.Points)
        {
            point.AddToCanvas(canvas);
        }
    }

    public Canvas? Canvas;
    public double Speed = 60;

    internal void SpawnItem()
    {
        for (int i = 0; i < LanesCount; i++)
        {
            var item = new Item(this) { Lane = i };
            Items[i].Enqueue(item);
        }
    }

    public Item? GetNextItem(Item currentItem)
    {
        var queue = Items[currentItem.Lane];
        var itemList = queue.Reverse().ToList(); // TODO OPTIMIZE!!!   !!!!
        var idx = itemList.IndexOf(currentItem);
        if (idx < itemList.Count - 1)
        {
            return itemList[idx + 1];
        }
        return null;
    }

    private readonly ConcurrentQueue<Item>[] Items;
    private Thread? Dispatcher;

    public Conveyor(int lanesCount)
    {
        LanesCount = lanesCount;
        LaneIndexes = Enumerable.Range(0, lanesCount).ToArray();

        SegmentLanes = new LinkedList<ConveyorSegmentLane>[lanesCount];
        PointLanes = new LinkedList<ConveyorPointLane>[lanesCount];
        PointAndSegmentLanes = new LinkedList<ILanePart>[lanesCount];

        Items = new ConcurrentQueue<Item>[lanesCount];
        
        foreach (int i in LaneIndexes)
        {
            SegmentLanes[i] = new();
            PointLanes[i] = new();
            PointAndSegmentLanes[i] = new();
            Items[i] = new();
        }
    }

    private static void ItemDispatcherThreadAction(object? obj)
    {
        if (obj is Conveyor conveyor)
        {
            DateTime time = DateTime.Now;
            while (conveyor.IsRunning)
            {
                var now = DateTime.Now;
                var diff = (now - time).TotalMilliseconds;
                time = now;
                foreach (int i in conveyor.LaneIndexes)
                {
                    foreach (var item in conveyor.Items[i].ToList()) // TODO hier muss das tolist weg - anderen datentypen wählen.
                    {
                        if (!item.Done)
                        {
                            item.AddAge(diff);
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }
    }

    // TODO add an out parameter for stalling
    internal Point GetItemLocation(Item item, out bool done, out LinkedListNode<ConveyorSegment> segmentNode, out LinkedListNode<ConveyorSegmentLane> segmentLane)
    {
        var actualAge = item.Age - item.StaleAge;
        var nextItem = GetNextItem(item);
        if (nextItem is not null)
        {
            var nextAge = nextItem.Age - nextItem.StaleAge;
            if (actualAge + Speed * 2.8 > nextAge) // this needs to be something depending on the speed and size of the items.
            {
                // collision -> avoid
                segmentLane = item.SegmentLane ?? Segments.First.Value.Lanes[item.Lane].Node;
                segmentNode = item.Segment ?? Segments.First;
                done = false;
                return item.Location;
            }
        }

        double length = actualAge / 1000 * Speed;
        done = false;
        segmentNode = item.Segment ?? Segments.First;
        segmentLane = item.SegmentLane ?? Segments.First.Value.Lanes[item.Lane].Node;

        while (segmentLane is not null && segmentLane.Value.EndLength < length)
        {
            segmentNode = segmentNode!.Next;
            segmentLane = segmentLane!.Next;
        }

        if (segmentLane is null)
        {
            done = true;
            segmentLane = Segments.Last.Value.Lanes[item.Lane].Node;
        }
        if (length < 0)
        {
            length = 0;
        }
        return segmentLane.Value.GetPointAbsolute(length);
    }
}