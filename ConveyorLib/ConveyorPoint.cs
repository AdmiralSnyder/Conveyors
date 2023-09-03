using UILib.Shapes;
//using System.Windows.Shapes;

namespace ConveyorLib;

public interface IElementsNode<T>
{
    public LinkedListNode<IPathPart> ElementsNode { get; }
}

public interface IListNode<T>
{
    public LinkedListNode<T> Node { get; }
}

public interface IAutomationOutByID
{
    string ID { get; }
}

public class ConveyorPoint : IConveyorCanvasable, IPathPart, ISelectObject, IElementsNode<IPathPart>, IListNode<ConveyorPoint>, IRefreshable, IAutomationOutByID
{
    public string ID => $"{Conveyor.Number}.{Number}";

    public string Text => $"Point {ID} ({Location})";

    public static Dictionary<string, ConveyorPoint> PointsByID = new();

    public static implicit operator ConveyorPoint(string id) => PointsByID.TryGetValue(id, out var cp) ? cp : null;

    public ConveyorPoint(Conveyor conveyor)
    {
        Conveyor = conveyor;
        Lanes = new ConveyorPointLane[conveyor.LanesCount];
        LaneStrategy = PointLaneStrategies.Curve;
        Number = Conveyor.Points.Count;
        PointsByID[ID] = this;
    }

    public bool IsLast { get; internal set; }
    public bool IsFirst { get; internal set; }

    private void PointLocationUpdated()
    {
        PointCircle?.SetCenterLocation(Location);
        ((ISelectObject)this).SetSelectionPoints(_Location);

        var (prev, next) = GetAdjacentSegments();

        PreparePoint();

        var prevP = prev?.GetPreviousPoint();
        var nextP = next?.GetNextPoint();

        prev?.SetEnd(Location);
        next?.SetStart(Location);

        bool move = false;

        if (!move)
        {
            prevP?.PreparePoint();
            nextP?.PreparePoint();

            prevP?.RebuildLanes2();
        }

        
        RebuildLanes2();
        if (!move)
        {
            nextP?.RebuildLanes2();
        }

        //if (prev?.GetPreviousPoint() is { } )
        //{
        //    prev.StartEnd = (prev.StartEnd.P1, Location);
        //}
        //if (next is { })
        //{
        //    next.StartEnd = (Location, next.StartEnd.P2);
        //}




        //(prev?.TryGetPreviousPoint(2) ?? this)?.RebuildLanes();


        //next?.GetAdjacentPoints().next?.RebuildLanes();

        // TODO this might better be a method of the lanes?

        //double len = 0d;
        //foreach (var i in Conveyor.LaneIndexes)
        //{
        //    if (prev is { })
        //    {
        //        len = prev.Lanes[i]?.EndLength ?? 0d;
        //    }
        //    var elNode = next?.Lanes[i]?.ElementsNode;
        //    while (elNode is { })
        //    {
        //        var element = elNode.Value;
        //        element.BeginLength = len;
        //        len = element.EndLength;
        //        elNode = elNode.Next;
        //    }
        //}

        // This is utterly dirty - the conveyor should be a listener on the point's locations - or rather, all locations...
        ((ISelectObject)Conveyor).SetSelectionPoints();

    }

    private Point _Location;
    public Point Location
    {
        get => _Location;
        set => Func.Setter(ref _Location, value, PointLocationUpdated);
    }

    public void RebuildLanes()
    {
        foreach (var lane in Lanes)
        {
            lane?.RebuildArc();
        }

        ElementsNode.Next?.Value?.RebuildLanes();
    }

    public void RebuildLanes2()
    {
        foreach (var lane in Lanes)
        {
            lane?.RebuildArc();
        }
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

    public ConveyorPointLane[] Lanes;

    public PointLaneStrategies LaneStrategy { get; }
    public int Number { get; }
    public IEllipse PointCircle { get; internal set; }
    public Conveyor Conveyor { get; }
    public LinkedListNode<ConveyorPoint> Node { get; internal set; }
    public LinkedListNode<IPathPart> ElementsNode { get; internal set; }

    public Vector IncomingVector { get; private set; }
    public Vector IncomingNorm { get; private set; }
    public Vector IncomingNormInversed { get; private set; }

    public Vector OutgoingVector { get; private set; }
    public Vector OutgoingNorm { get; private set; }

    public Angle Angle { get; private set; }

    public Angle AbsoluteAngle { get; private set; }

    public Angle IncomingAngle { get; private set; }
    public Angle OutgoingAngle { get; private set; }

    public bool IsClockwise { get; private set; }
    public bool IsStraight { get; private set; }

    private Point[] SelectionBoundsPoints = new Point[1];
    public Point[] GetSelectionBoundsPoints() => SelectionBoundsPoints;


    public ISelectObject? SelectionParent => Conveyor;

    public void AddToCanvas(IConveyorCanvasInfo canvasInfo)
    {
        PointCircle = canvasInfo.ShapeProvider.CreateConveyorPointEllipse(Location, IsFirst, IsLast, IsClockwise, IsStraight);
        PointCircle.Tag = this;
        canvasInfo.AddToCanvas(PointCircle);

        if (IsFirst || IsLast) return;
        foreach (var lane in Lanes)
        {
            lane.AddToCanvas(canvasInfo);
        }
    }

    internal void CreateLanes()
    {
        if (IsFirst || IsLast) return;

        foreach (var i in Conveyor.LaneIndexes)
        {
            var lane = Lanes[i] = new(this);
            lane.Lane = i;
        }
    }

    internal void PreparePoint()
    {
        var prevNode = Node.Previous;
        var nextNode = Node.Next;

        if (prevNode is not null && nextNode is not null)
        {

            var prevLocation = ((ConveyorPoint)prevNode.Value).Location;
            var nextLocation = ((ConveyorPoint)nextNode.Value).Location;

            IncomingVector = new(prevLocation, Location);
            IncomingNorm = IncomingVector.Normalize();
            IncomingNormInversed = IncomingNorm.Inverse();

            OutgoingVector = new(Location, nextLocation);
            OutgoingNorm = OutgoingVector.Normalize();

            var dotInX = IncomingNorm.DotProduct(Maths.XAxisV1);
            var dotOutX = OutgoingNorm.DotProduct(Maths.XAxisV1);

            IncomingAngle = Math.Acos(dotInX).Radians();
            OutgoingAngle = Math.Acos(dotOutX).Radians();

            Angle = Maths.AngleBetween(IncomingNormInversed, OutgoingNorm);
            AbsoluteAngle = Math.Abs(Angle.Radians).Radians();

            IsClockwise = Angle.Radians < 0;
            IsStraight = Angle.IsStraight;
        }

        ((IRefreshable)this).NotifyRefresh();
    }

    public void RegisterLanes()
    {
        if (IsFirst || IsLast) return;

        foreach (var lane in Lanes)
        {
            lane.ElementsNode = Conveyor.PointAndSegmentLanes[lane.Lane].AddLast(lane);
            lane.Node = Conveyor.PointLanes[lane.Lane].AddLast(lane);
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

    public (ConveyorSegment? prev, ConveyorSegment? next) GetAdjacentSegments() => (
        ElementsNode.Previous?.Value as ConveyorSegment,
        ElementsNode.Next?.Value as ConveyorSegment
    );
}

public enum PointLaneStrategies
{
    StraightLineSegment,
    Curve,
}