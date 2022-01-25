using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1;

public class Item : ISelectObject
{
    public string DebugText => $"Item {Number}";

    public string Text => $"Item {Number}";

    [ThreadStatic]
    public static int Num = 0;

    public int Lane { get; }
    public Item(Conveyor conveyor, int lane)
    {
        Lane = lane;
        Conveyor = conveyor;
        Shape = new Ellipse() { Width = 10, Height = 10, Fill = Brushes.Blue, Tag = this };
        Conveyor.Canvas.Children.Add(Shape);
        var layer = AdornerLayer.GetAdornerLayer(Shape);
        layer.Add(new TextAdorner(Shape));
        AddAge(0);
        Number = Num++;
    }

    public int Number { get; }
    public Shape Shape { get; }

    private double _Age;
    public double Age => _Age;

    public void AddAge(double offset)
    {
        var oldlocation = Location;
        _Age += offset;
        var newLocation = Conveyor.GetItemLocation(this, out var done, out var segment, out var segmentLane, out var staleAge);
        _Age -= staleAge;
        StaleAge += staleAge;
        if (oldlocation == newLocation)
        {
            Location = oldlocation;
        }
        else
        {
            SegmentLane = segmentLane;
            Segment = segment;
            Location = newLocation;
        }

        if (done)
        {
            Done = true;
        }
    }

    //public double Age
    //{
    //    get => _Age;
    //    set
    //    {
    //        _Age = value;
    //        Location = Conveyor.GetItemLocation(this, out var done, out var segment);
    //        Segment = segment;
    //        if (done)
    //        {
    //            Done = true;
    //        }
    //    }
    //}

    public double StaleAge = 0;

    public bool Moving { get; set; }
    public bool Done { get; set; }
    public Conveyor Conveyor { get; set; }

    public LinkedListNode<ConveyorSegmentLane> SegmentLane { get; set; }
    public LinkedListNode<ConveyorSegment> Segment { get; set; }

    private Point _Location;
    public Point Location
    {
        get => _Location;
        set
        {
            Func.Setter(ref _Location, value, () =>
            {
                Shape.Dispatcher.BeginInvoke(() =>
                {
                    Canvas.SetLeft(Shape, value.X - Shape.Width / 2);
                    Canvas.SetTop(Shape, value.Y - Shape.Height / 2);
                });
            });
        }
    }
}
