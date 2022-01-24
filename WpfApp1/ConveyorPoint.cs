using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfLib;
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
        });
    }

    public ConveyorPointLane[] Lanes;

    public PointLaneStrategies LaneStrategy { get; }
    public Ellipse PointCircle { get; internal set; }
    public Conveyor Conveyor { get; }
    public LinkedListNode<ConveyorPoint> Node { get; internal set; }
    public LinkedListNode<IPathPart> ElementsNode { get; internal set; }

    public void AddToCanvas(CanvasInfo canvasInfo)
    {
        PointCircle = canvasInfo.ShapeProvider.CreateConveyorPointEllipse(Location, IsFirst, IsLast);
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