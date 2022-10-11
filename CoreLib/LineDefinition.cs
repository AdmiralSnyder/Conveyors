namespace CoreLib.Maths;

public interface IDefinition<TSource>
{
    TSource GetSource();
    void ApplySource(TSource source);
}

public class SimpleDefinition<TSource> : IDefinition<TSource>
{
    public TSource Source { get; set; }
    public void ApplySource(TSource source) => Source = source;

    public TSource GetSource() => Source;
}

public class LineDefinition : IDefinition<TwoPoints>
{
    public LineDefinition() { }

    public LineDefinition(TwoPoints twoPoints) => ApplySource(twoPoints);

    public TwoPoints GetSource() => RefPoints;

    public void ApplySource(TwoPoints twoPoints)
    {
        if (twoPoints.PointsAreEqual()) throw new MathsException("line is degenerated");

        IsDegenerated = false;
        bool isVertical = twoPoints.P1.X == twoPoints.P2.X;
        var vector = twoPoints.P2 - twoPoints.P1;
        ReferencePoint1 = twoPoints.P1;
        ReferencePoint2 = twoPoints.P2;
        RefPoints = twoPoints;
        Vector = vector;
        IsVertical = isVertical;
        Maths.GetSlope(vector, out var slope);
        Slope = slope;
        OffsetY = isVertical ? double.NaN : Maths.GetOffsetY(twoPoints.P2, slope);
    }

    public LineDefinition(Point pointOnLine, Vector lineVector) : this((pointOnLine, pointOnLine + lineVector)) { }

    public Point ReferencePoint1 { get; private set; }
    public Point ReferencePoint2 { get; private set; }
    public TwoPoints RefPoints { get; private set; }
    public Vector Vector { get; private set; }
    public bool IsDegenerated { get; private set; }
    public bool IsVertical { get; private set; }
    public double Slope { get; private set; }
    public double OffsetY { get; private set; }
}
