using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using WpfLib;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace ConveyorLib;

public class ConveyorShapeProvider : ShapeProvider
{
    public Ellipse CreateTempPoint(Point point) => new Ellipse()
    {
        Width = 3,
        Height = 3,
        Fill = Brushes.Magenta,
    }.SetCenterLocation(point);

    public Shape CreatePoint(Point point)=> new Ellipse()
    {
        Width = 3,
        Height = 3,
        Fill = Brushes.Magenta,
    }.SetCenterLocation(point).WithSelectBehaviour();

    public Shape CreateCircle(Point center, double radius) => new Ellipse()
    {
        Width = radius * 2 + 1,
        Height = radius * 2 + 1,
        Stroke = Brushes.Magenta,
    }.SetCenterLocation(center).WithSelectBehaviour();

    public Line CreateTempLine(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.Stroke = Brushes.Magenta;
        return line;
    }

    public Line CreateConveyorPositioningLine(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.Stroke = Brushes.Black;
        line.StrokeThickness = 2;
        return line;
    }

    public Line CreateConveyorSegmentLine(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.Stroke = Brushes.Red;
        line.StrokeThickness = 2;
        return line;
    }

    public Line CreateLineSegment(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.Stroke = Brushes.Beige;
        line.StrokeThickness = 2;
        return line;
    }

    public Line CreateDebugLineSegment(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.Stroke = Brushes.Magenta;
        line.StrokeThickness = 1.5;
        return line;
    }

    public Line CreateDebugThinLineSegment(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.Stroke = Brushes.Magenta;
        line.StrokeThickness = 0.5;
        return line;
    }

    public Line CreateLine(TwoPoints points)
    {
        var vector = points.P2 - points.P1;
        var start = points.P1;
        var end = points.P2;
        //       -----_-----
        var len = points.Length();
        if (len < 1000)
        {
            var factor = 1000 / len;
            start = points.P1 - vector.Multiply(factor);
            end = points.P2 + vector.Multiply(factor);

        }
        var line = PrepareLine((start, end));
        line.Stroke = Brushes.Beige;
        line.StrokeThickness = 2;
        return line;
    }

    public Line CreateConveyorSegmentLaneLine(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.Stroke = Brushes.White;
        line.StrokeThickness = 1;
        return line;
    }


    public Ellipse CreateConveyorPointEllipse(Point point, bool isFirst, bool isLast, bool isClockwise, bool isStraight, double size = 4d) => new Ellipse()
    {
        Width = size,
        Height = size,
        Fill
        = isLast ? Brushes.Red
        : isFirst ? Brushes.Cyan
        : isClockwise ? Brushes.Purple
        : isStraight ? Brushes.Peru
        : Brushes.Blue,
    }.SetCenterLocation(point).WithSelectBehaviour();


    public Path CreateConveyorPointPath(PathGeometry geometry, bool isLeft) => new Path()
    {
        Data = geometry,
        Stroke = isLeft ? Brushes.Plum : Brushes.Tomato,
    }.WithSelectBehaviour();

    public Path CreateCircleSectorArc(PathGeometry geometry, bool isLeft) => new Path()
    {
        Data = geometry,
        Stroke = isLeft ? Brushes.Orange : Brushes.Teal,
        StrokeThickness = 2,
    }.WithSelectBehaviour();

    public Ellipse CreatePointMoveCircle(Point location, Action<Shape> leftClickAction)
    {
        const double Size = 15d;
        Ellipse result = new()
        {
            Width = Size,
            Height = Size,
            Stroke = Brushes.BurlyWood,
            StrokeThickness = 3,
            Fill = Brushes.Transparent,
            Cursor = Cursors.Hand
        };

        result.ApplyMouseBehavior(leftClickAction, MouseAction.LeftClick);
        result.SetCenterLocation(location);
        return result;
    }

    private const double ItemSize = 3;

    public Ellipse CreateConveyorItemEllipse() => new() { Width = ItemSize, Height = ItemSize, Fill = Brushes.Blue };

    internal Shape CreateFillet(TwoPoints points, double radius)
    {
        var pg = new PathGeometry();

        pg.Figures.Add(new()
        {
            StartPoint = points.P1,
            Segments = { new ArcSegment(points.P2, new(radius, radius), 0, false, SweepDirection.Clockwise, true) }
        });

        return CreateCircleSectorArc(pg, true);
    }
}
