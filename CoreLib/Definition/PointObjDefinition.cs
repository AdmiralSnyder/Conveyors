namespace CoreLib.Definition;

public class PointObjDefinition : IDefinition<Point>
{
    public PointObjDefinition() { }
    public PointObjDefinition(Point point) => ApplySource(point);


    public Point Point { get; set; }
    public Point GetSource() => Point;

    public void ApplySource(Point source)
    {
        // TODO hier Checks ggf.
        Point = source;
    }

    public bool IsSelectionMatch(Vector point) => Maths.Maths.IsSelectionMatch(Point, point);

}
