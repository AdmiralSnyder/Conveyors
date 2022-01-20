using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

public interface IPathPart
{
    void RegisterLanes();
}

public class ConveyorPoint : ICanvasable, IPathPart
{
    public ConveyorPoint(Conveyor conveyor)
    {
        Conveyor = conveyor;
        Lanes = new ConveyorPointLane[conveyor.LanesCount];
    }

    public bool IsLast { get; internal set; }
    public bool IsFirst { get; internal set; }
    public double Y { get; internal set; }
    public double X { get; internal set; }

    public ConveyorPointLane[] Lanes;

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

public class ConveyorPointLane : ICanvasable, ILanePart
{
    public ConveyorPointLane(ConveyorPoint point) => Point = point;
    public Path Arc { get; set; }
    public int Lane { get; internal set; }
    public LinkedListNode<ILanePart> ElementNode { get; internal set; }
    public ConveyorPoint Point { get; }

    public void AddToCanvas(Canvas canvas)
    {
        canvas.Children.Add(Arc);
    }

    internal void Prepare()
    {
        if (Point.IsFirst || Point.IsLast) return;
        var prevLine = ((ConveyorSegmentLane)ElementNode.Previous.Value).Line;
        var nextLine = ((ConveyorSegmentLane)ElementNode.Next.Value).Line;
        var pg = new PathGeometry()
        { };

        Arc = new()
        {
            Stroke = Brushes.Plum,
        };
        Arc.Data = pg;
        pg.Figures.Add(new()
        {
            StartPoint = new(prevLine.X2, prevLine.Y2),
            Segments = { new LineSegment(new(nextLine.X1, nextLine.Y1), true)}
        });

    }
}