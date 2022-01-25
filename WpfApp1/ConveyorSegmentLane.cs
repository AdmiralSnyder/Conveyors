using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using WpfLib;

namespace WpfApp1;

public interface ILanePart
{
    double BeginLength { set; }
    double EndLength { get; }
}

public interface IDebugText
{
    string DebugText { get; }
}

[DebuggerDisplay($"{"SegLane"} ({{{nameof(ConveyorSegmentLane.DebugText)}}})")]
public class ConveyorSegmentLane : ICanvasable, ILanePart, IDebugText, ISelectObject
{
    public string Text => "KEkse";
    public ConveyorSegmentLane(double beginLength, int lane, ConveyorSegment segment)
    {
        BeginLength = beginLength;
        Lane = lane;
        Segment = segment;
        Rebuild();
    }

    public void Rebuild() => StartEnd = GetLanePoints(Segment.StartEnd, Lane);

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


    private TwoPoints OffsetLanePoints(TwoPoints original, double offset)
    {
        var origVect = original.Vector();
        var origNormalVect = origVect.Normalize();
        Vector offsetNormalVect = (-origNormalVect.Y, origNormalVect.X);
        var offsetVect = offsetNormalVect.Multiply(offset);
        return original.Add(offsetVect);
    }

    public LinkedListNode<ConveyorSegmentLane> Node { get; set; }
    private TwoPoints _StartEnd;
    private TwoPoints StartEnd 
    {
        get => _StartEnd;
        set => Func.Setter(ref _StartEnd, value, () =>
        {
            Length = StartEnd.Length();
            EndLength = BeginLength + Length;
            UnitVector = StartEnd.Vector().Normalize(Length);
            StartPoint = StartEnd.P1;
            EndPoint = StartEnd.P2;
            if (Line is not null)
            {
                Line.X1 = StartEnd.P1.X;
                Line.Y1 = StartEnd.P1.Y;
                Line.X2 = StartEnd.P2.X;
                Line.Y2 = StartEnd.P2.Y;
            }
        });
    }
    public Line? Line { get; private set; }

    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }
    public double EndLength { get; set; }

    private double _BeginLength;
    public double BeginLength 
    {
        get => _BeginLength;
        set => Func.Setter(ref _BeginLength, value, UpdateEndLength);
    }

    private void UpdateEndLength() => EndLength = BeginLength + Length;

    public void AddToCanvas(CanvasInfo canvasInfo)
    {
        Line = canvasInfo.ShapeProvider.CreateConveyorSegmentLaneLine(StartEnd);
        Line.Tag = this;
        
        canvasInfo.Canvas.Children.Add(Line);
    }

    private double _Length;
    public double Length 
    {
        get => _Length;
        set => Func.Setter(ref _Length, value, UpdateEndLength);
    }
    public Point UnitVector { get; internal set; }
    public LinkedListNode<ILanePart> ElementNode { get; internal set; }
    public int Lane { get; internal set; }
    public ConveyorSegment Segment { get; }

    public string DebugText => $"{Segment.Conveyor.Number}.{Segment.Number}.{Lane}";

    internal Point GetPointAbsolute(double length, bool overshoot = false)
    {
        length -= BeginLength;
        if (length < Length || overshoot)
        {
            var mult = length;// TODO hier ggf. auf spline umstellen
            return new(UnitVector.X * mult + StartPoint.X, UnitVector.Y * mult + StartPoint.Y);
        }
        else
        {
            return EndPoint;
        }
    }
}
