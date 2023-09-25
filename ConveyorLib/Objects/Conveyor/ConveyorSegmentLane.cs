using System.Diagnostics;
using CoreLib.Definition;
using UILib.Shapes;

namespace ConveyorLib.Objects.Conveyor;

[DebuggerDisplay($"{"SegLane"} ({{{nameof(DebugText)}}})")]
public class ConveyorSegmentLane : IConveyorCanvasable, ILanePart, IDebugText, ISelectObject, IRefreshable
{
    public string ID { get; } = Guid.NewGuid().ToString();

    public string Text => $"Lane ({DebugText})";

    public ConveyorSegmentLane(int lane, ConveyorSegment segment)
    {
        LaneNumber = lane;
        Segment = segment;
        Rebuild();
    }

    public ILine? Line { get; private set; }

    public double EndLength { get; set; }

    private double _BeginLength;
    public double BeginLength
    {
        get => _BeginLength;
        set => Func.Setter(ref _BeginLength, value, UpdateEndLength);
    }

    private double _Length;
    public double Length
    {
        get => _Length;
        set => Func.Setter(ref _Length, value, UpdateEndLength);
    }
    public Vector UnitVector { get; internal set; }
    public LinkedListNode<ILanePart> ElementsNode { get; internal set; }
    public int LaneNumber { get; internal set; }
    public ConveyorSegment Segment { get; }

    public string DebugText => $"{Segment.Conveyor.Number}.{Segment.Number}.{LaneNumber}";


    public void Rebuild() => StartEnd = GetLanePoints(Segment.StartEnd, LaneNumber);

    private TwoPoints GetLanePoints(TwoPoints original, int idx)
    {
        //1    0
        //2   -0.5  0.5
        //3   -1  0  1
        //4   -1.5 -0.5  0.6  1.5
        var laneCount = Segment.Conveyor.LanesCount;
        var steps = laneCount - 1;
        var leftmost = -steps / 2.0;
        var offset = leftmost + idx;
        var scaledOffset = offset * ConveyorSegment.LineDistance;

        return OffsetLanePoints(original, scaledOffset);
    }


    private static TwoPoints OffsetLanePoints(TwoPoints original, double offset)
    {
        var origVect = original.Vector();
        var origNormalVect = origVect.Normalize();
        Vector offsetNormalVect = (-origNormalVect.Y, origNormalVect.X);
        var offsetVect = offsetNormalVect.Multiply(offset);
        return original.Add(offsetVect);
    }

    public LinkedListNode<ConveyorSegmentLane> Node { get; set; }

    private Point _StartPoint;
    public Point StartPoint
    {
        get => _StartPoint;
        set => Func.Setter(ref _StartPoint, value, UpdateStartEnd);
    }

    private Point _EndPoint;
    public Point EndPoint
    {
        get => _EndPoint;
        set => Func.Setter(ref _EndPoint, value, UpdateStartEnd);
    }

    private void UpdateStartEnd() => StartEnd = (StartPoint, EndPoint);


    private TwoPoints _StartEnd;
    private TwoPoints StartEnd
    {
        get => _StartEnd;
        set => Func.Setter(ref _StartEnd, value, () =>
        {
            UpdateLength();
            UnitVector = StartEnd.Vector().Normalize(Length);
            _StartPoint = StartEnd.P1;
            _EndPoint = StartEnd.P2;
            if (Line is not null)
            {
                Line.X1 = StartEnd.P1.X;
                Line.Y1 = StartEnd.P1.Y;
                Line.X2 = StartEnd.P2.X;
                Line.Y2 = StartEnd.P2.Y;
            }

            ((ISelectObject)this).SetSelectionPoints(StartEnd.P1, StartEnd.P2);
        });
    }

    void UpdateLength()
    {
        Length = StartEnd.Length();
        EndLength = BeginLength + Length;
    }

    private Point[] SelectionBoundsPoints = new Point[2];

    public Point[] GetSelectionBoundsPoints() => SelectionBoundsPoints;

    public ISelectObject? SelectionParent => Segment;

    private void UpdateEndLength() => EndLength = BeginLength + Length;

    public void AddToCanvas(IConveyorCanvasInfo canvasInfo)
    {
        Line = canvasInfo.ShapeProvider.CreateConveyorSegmentLaneLine(StartEnd);
        Line.Tag = this;
        canvasInfo.AddToCanvas(Line);
    }

    public Point GetPointAbsolute(double length, bool overshoot = false) => StartEnd.GetPointOnLine(length - BeginLength, UnitVector, Length, overshoot);

    public bool IsSelectionMatch(Vector point) => Maths.IsSelectionMatch(new LineDefinition(StartEnd), point);
}
