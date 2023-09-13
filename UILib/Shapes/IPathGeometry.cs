namespace UILib.Shapes;
public enum ArcSweepDirections
{
    CounterClockwise = 0, //SweepDirection.Counterclockwise,
    Clockwise = 1, //SweepDirection.Clockwise,
}

public interface IPathGeometry
{
    void AddArcFigure(Point startPoint, Point EndPoint, double radius, double degrees, bool isLargeArc, ArcSweepDirections sweepDirection);
    
    void AddLineFigure(Point start, Point end);
    void ClearFigures();
}
