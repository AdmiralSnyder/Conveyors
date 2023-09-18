using PointDef;
using System.Windows.Media;

namespace WpfLib;

public static class WpfFunc
{
    public static V2d AsPoint(this System.Windows.Point point) => (point.X, point.Y);
    public static System.Windows.Point AsPoint(this V2d point) => new(point.X, point.Y);

    public static Shape SetLocationWpf(this Shape shape, Point location)
    {
        Canvas.SetLeft(shape, location.X);
        Canvas.SetTop(shape, location.Y);
        return shape;
    }

    public static bool TryGetLocationWpf(this Shape shape, out Point location)
    {
        var x = Canvas.GetLeft(shape);
        var y = Canvas.GetTop(shape);
        if (double.IsNaN(x) || double.IsNaN(y))
        {
            location = default;
            return false;
        }
        else
        {
            location = (x, y);
            return true;
        }
    }

    public static Line SetLocationWpf(this Line line, TwoPoints points)
    {
        if (points.P1.X != line.X1)
        {
            line.X1 = points.P1.X;
        }
        if (points.P1.Y != line.Y1)
        {
            line.Y1 = points.P1.Y;
        }
        if (points.P2.X != line.X2)
        {
            line.X2 = points.P2.X;
        }
        if (points.P2.Y != line.Y2)
        {
            line.Y2 = points.P2.Y;
        }

        return line;
    }

    public static V2d GetSizeWpf<TShape>(this TShape shape) where TShape : Shape => (shape.Width, shape.Height);

    public static System.Drawing.Color AsColor(this System.Windows.Media.Color color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
    public static System.Windows.Media.Color AsColor(this System.Drawing.Color color) => System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
    public static SolidColorBrush? AsBrush(this System.Drawing.Color? color) => color.HasValue ? new SolidColorBrush(color.Value.AsColor()) : null;

    public static System.Drawing.Color? GetBrushColor(this Brush brush) => brush is SolidColorBrush scb ? (scb.Color.AsColor()) : null;
}
