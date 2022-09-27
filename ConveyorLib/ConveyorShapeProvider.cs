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
    

    public Line CreateConveyorPositioningLine(TwoPoints points)
    {
        var line = CreateLine(points);
        line.Stroke = Brushes.Black;
        line.StrokeThickness = 2;
        return line;
    }

    public Line CreateConveyorSegmentLine(TwoPoints points)
    {
        var line = CreateLine(points);
        line.Stroke = Brushes.Red;
        line.StrokeThickness = 2;
        return line;
    }

    public Line CreateConveyorSegmentLaneLine(TwoPoints points)
    {
        var line = CreateLine(points);
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
        };
        result.Cursor = Cursors.Hand;

        result.ApplyMouseBehaviour(leftClickAction, MouseAction.LeftClick);
        result.SetCenterLocation(location);
        return result;
    }

    private const double ItemSize = 3;

    public Ellipse CreateConveyorItemEllipse() => new() { Width = ItemSize, Height = ItemSize, Fill = Brushes.Blue };
}
