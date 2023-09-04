namespace UILib.Shapes;
public enum ArcSweepDirections
{
    CounterClockwise = 0, //SweepDirection.Counterclockwise,
    Clockwise = 1, //SweepDirection.Clockwise,
}

public interface IPathGeometry
{
    void AddArcFigure(Vector prevEnd, Vector nextStart, double radius, double degrees, bool isLargeArc, ArcSweepDirections sweepDirection);
    
    void AddLineFigure(Vector prevEnd, Vector nextStart);
    void ClearFigures();
}
