using System.Windows.Shapes;
using WpfLib;
using System.Windows.Media;
using SDColor = System.Drawing.Color;
using System.Windows.Input;
using System.Windows.Documents;
using UILib.Shapes;
using WpfLib.Shapes;

namespace ConveyorLib.Wpf;

public class WpfConveyorShapeProvider : ShapeProvider, IConveyorShapeProvider
{
    public IEllipse CreateTempPoint(Point point) => new WpfEllipse(new()
    {
        Width = 3,
        Height = 3,
        Fill = Brushes.Magenta,
    }).SetCenterLocation(point);

    public IShape CreatePoint(Point point) => new WpfEllipse(new()
    {
        Width = 3,
        Height = 3,
        Fill = Brushes.Magenta,
    }).SetCenterLocation(point).WithSelectBehaviour();

    public IShape CreateCircle(Point center, double radius) => new WpfEllipse(new()
    {
        Width = radius * 2 + 1,
        Height = radius * 2 + 1,
        Stroke = Brushes.Magenta,
    }).SetCenterLocation(center).WithSelectBehaviour();

    public ILine CreateTempLine(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.StrokeColor = SDColor.Magenta;
        return line;
    }

    public ILine CreateConveyorPositioningLine(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.StrokeColor = SDColor.Black;
        line.StrokeThickness = 2;
        return line;
    }

    public ILine CreateConveyorSegmentLine(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.StrokeColor = SDColor.Red;
        line.StrokeThickness = 2;
        return line;
    }

    public ILine CreateLineSegment(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.StrokeColor = SDColor.Beige;
        line.StrokeThickness = 2;
        return line;
    }

    public ILine CreateDebugLineSegment(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.StrokeColor = SDColor.Magenta;
        line.StrokeThickness = 1.5;
        return line;
    }

    public ILine CreateDebugThinLineSegment(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.StrokeColor = SDColor.Magenta;
        line.StrokeThickness = 0.5;
        return line;
    }

    public ILine CreateLine(TwoPoints points)
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
        line.StrokeColor = SDColor.Beige;
        line.StrokeThickness = 2;
        return line;
    }

    public ILine CreateConveyorSegmentLaneLine(TwoPoints points)
    {
        var line = PrepareLine(points);
        line.StrokeColor = SDColor.White;
        line.StrokeThickness = 1;
        return line;
    }


    public IEllipse CreateConveyorPointEllipse(Point point, bool isFirst, bool isLast, bool isClockwise, bool isStraight, double size = 4d) => new WpfEllipse(new Ellipse()
    {
        Width = size,
        Height = size,
        Fill
        = isLast ? Brushes.Red
        : isFirst ? Brushes.Cyan
        : isClockwise ? Brushes.Purple
        : isStraight ? Brushes.Peru
        : Brushes.Blue,
    }).SetCenterLocation(point).WithSelectBehaviour();


    public IPath CreateConveyorPointPath(IPathGeometry geometry, bool isLeft) => new WpfPath(new()
    {
        Data = ((WpfPathGeometry)geometry).BackingObject,
        Stroke = isLeft ? Brushes.Plum : Brushes.Tomato,
    }).WithSelectBehaviour();

    public IPath CreateCircleSectorArc(PathGeometry geometry, bool isLeft) => new WpfPath(new()
    {
        Data = geometry,
        Stroke = isLeft ? Brushes.Orange : Brushes.Teal,
        StrokeThickness = 2,
    }).WithSelectBehaviour();

    public IEllipse CreatePointMoveCircle(Point location, Action<IShape> leftClickAction)
    {
        const double Size = 15d;
        IEllipse result = new WpfEllipse(new()
        {
            Width = Size,
            Height = Size,
            Stroke = Brushes.BurlyWood,
            StrokeThickness = 3,
            Fill = Brushes.Transparent,
            Cursor = Cursors.Hand
        });

        result.ApplyMouseBehavior(leftClickAction, MouseAction.LeftClick);
        result.SetCenterLocation(location);
        return result;
    }

    private const double ItemSize = 3;

    public IEllipse CreateConveyorItemEllipse() => new WpfEllipse(new() { Width = ItemSize, Height = ItemSize, Fill = Brushes.Blue });

    public IShape CreateFillet(TwoPoints points, double radius)
    {
        var pg = new PathGeometry();

        pg.Figures.Add(new()
        {
            StartPoint = points.P1.AsPoint(),
            Segments = { new ArcSegment(points.P2.AsPoint(), new(radius, radius), 0, false, SweepDirection.Clockwise, true) }
        });

        return CreateCircleSectorArc(pg, true);
    }


    public void AddAdornedShapeLayer(IShape shape)
    {
        var layer = AdornerLayer.GetAdornerLayer(((WpfShape)shape).BackingShape);
        layer.Add(new ItemTextAdorner(((WpfShape)shape).BackingShape));
    }
}