using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using WpfLib;

namespace WpfApp1;

public interface ILanePart
{ }

public interface IDebugText
{
    string DebugText { get; }
}

[DebuggerDisplay($"{"SegLane"} ({{{nameof(ConveyorSegmentLane.DebugText)}}})")]
public class ConveyorSegmentLane : ICanvasable, ILanePart, IDebugText
{
    public ConveyorSegmentLane(double beginLength, TwoPoints startEnd, int lane, ConveyorSegment segment)
    {
        BeginLength = beginLength;
        StartEnd = startEnd;
        Lane = lane;
        Segment = segment;
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
    public double BeginLength { get; set; }

    public void AddToCanvas(CanvasInfo canvasInfo)
    {
        Line = canvasInfo.ShapeProvider.CreateConveyorSegmentLaneLine(StartEnd);
        Line.Tag = this;
        
        canvasInfo.Canvas.Children.Add(Line);
    }

    public double Length { get; private set; }
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
