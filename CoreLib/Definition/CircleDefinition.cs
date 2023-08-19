namespace CoreLib.Definition;
using CoreLib.Maths;

public class CircleDefinition : IDefinition<CircleDefinitionSource>
{
    public CircleDefinitionKinds Kind;
    public CircleDefinition() { }

    public CircleDefinition((Point Center, double Radius) centerRadius)
    {
        ApplySource((centerRadius.Center, centerRadius.Radius, CircleDefinitionKinds.CenterRadius,
            CenterRadius.Center.Add((CenterRadius.Radius, 0)),
            CenterRadius.Center.Add((-CenterRadius.Radius, 0)),
            CenterRadius.Center.Add((0, CenterRadius.Radius))));
    }

    public CircleDefinition(Point center, double radius) : this((center, radius)) { }

    public CircleDefinition(Point diameter1, Point diameter2)
    {
        if (!Maths.GetCircleInfoByDiameter((diameter1, diameter2), out var centerRadius, out var thirdPoint)) throw new MathsException("Degenerated Circle");
        ApplySource((centerRadius.Center, centerRadius.Radius, CircleDefinitionKinds.DiameterPoints, diameter1, diameter2, thirdPoint));
    }

    public CircleDefinition(Point point1, Point point2, Point point3)
    {
        if (!Maths.GetCircleInfoByThreePoints((point1, point2, point3), out var centerRadius)) throw new MathsException("degenerated cirlce");
        ApplySource((centerRadius.Center, centerRadius.Radius, CircleDefinitionKinds.ThreePoints, point1, point2, point3));
    }

    public CircleDefinitionSource GetSource() => (CenterRadius.Center, CenterRadius.Radius, Kind, ThreePoints.Point1, ThreePoints.Point2, ThreePoints.Point3);

    public (Point Center, double Radius) CenterRadius { get; private set; }
    public TwoPoints DiameterPoints { get; private set; }
    public (Point Point1, Point Point2, Point Point3) ThreePoints { get; private set; }

    public bool IsDegenerated { get; private set; }

    public void ApplySource(CircleDefinitionSource source)
    {
        // TODO hier Checks ggf.
        IsDegenerated = false;
        CenterRadius = (source.Center, source.Radius);
        Kind = source.Kind;
        switch (Kind)
        {
            case CircleDefinitionKinds.CenterRadius:
            case CircleDefinitionKinds.DiameterPoints:
                {
                    DiameterPoints = (source.P1, source.P2);
                    break;
                }
            case CircleDefinitionKinds.ThreePoints:
                {
                    DiameterPoints = (source.Center.Add((source.Radius, 0)), source.Center.Add((source.Radius, 0)));
                    break;
                }
        }
        ThreePoints = (source.P1, source.P2, source.P3);
    }
}

public enum CircleDefinitionKinds
{
    None,
    CenterRadius,
    DiameterPoints,
    ThreePoints,
}

public record struct CircleDefinitionSource(Vector Center, double Radius, CircleDefinitionKinds Kind, Vector P1, Vector P2, Vector P3)
{
    public static implicit operator CircleDefinitionSource((Vector Center, double Radius, CircleDefinitionKinds Kind, Vector P1, Vector P2, Vector P3) value)
    {
        return new CircleDefinitionSource(value.Center, value.Radius, value.Kind, value.P1, value.P2, value.P3);
    }
    public static implicit operator CircleDefinitionSource((Vector Center, double Radius) centerRadius)
    {
        if (!Maths.GetCircleInfoByCenterRadius(centerRadius, out var threePoints)) throw new MathsException("invalid circle");
        return new CircleDefinitionSource(centerRadius.Center, centerRadius.Radius,
            CircleDefinitionKinds.CenterRadius, threePoints.point1, threePoints.point2, threePoints.point3);
    }

    public static implicit operator CircleDefinitionSource((Vector Point1, Vector Point2) diameter)
    {
        if (!Maths.GetCircleInfoByDiameter(diameter, out var centerRadius, out var thirdPoint)) throw new MathsException("invalid circle");
        return new CircleDefinitionSource(centerRadius.Center, centerRadius.Radius,
            CircleDefinitionKinds.DiameterPoints, diameter.Point1, diameter.Point2, thirdPoint);
    }

    public static implicit operator CircleDefinitionSource((Vector Point1, Vector Point2, Vector Point3) threePoints)
    {
        if (!Maths.GetCircleInfoByThreePoints(threePoints, out var centerRadius)) throw new MathsException("invalid circle");
        return new CircleDefinitionSource(centerRadius.Center, centerRadius.Radius,
            CircleDefinitionKinds.ThreePoints, threePoints.Point1, threePoints.Point2, threePoints.Point3);
    }
}