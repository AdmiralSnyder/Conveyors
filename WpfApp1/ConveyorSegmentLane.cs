using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WpfApp1;

public interface ILanePart
{ }

public class ConveyorSegmentLane : ICanvasable, ILanePart
{
    public ConveyorSegmentLane(double beginLength, Line line)
    {
        BeginLength = beginLength;
        Line = line;
    }

    public LinkedListNode<ConveyorSegmentLane> Node { get; set; }
    private Line? _Line;
    public Line? Line
    {
        get => _Line;
        set
        {
            _Line = value;
            _Line!.Tag = this;
            Length = _Line.Length();
            EndLength = BeginLength + Length;
            UnitVector = new((_Line.X2 - _Line.X1) / Length, (_Line.Y2 - _Line.Y1) / Length);
            StartPoint = new(_Line.X1, _Line.Y1);
            EndPoint = new(_Line.X2, _Line.Y2);
        }
    }

    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }
    public double EndLength { get; set; }
    public double BeginLength { get; set; }

    public void AddToCanvas(Canvas canvas) => canvas.Children.Add(Line);

    public double Length { get; private set; }
    public Point UnitVector { get; internal set; }
    public LinkedListNode<ILanePart> ElementNode { get; internal set; }
    public int Lane { get; internal set; }

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
