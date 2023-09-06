namespace PointDef.twopoints;

public struct TwoPoints<TVect>
{
    public TwoPoints(TVect p1, TVect p2) => (P1, P2) = (p1, p2);
    public TwoPoints((TVect x, TVect y) tuple) => (P1, P2) = tuple;

    public TVect P1 { get; set; }
    public TVect P2 { get; set; }

    public static implicit operator TwoPoints<TVect>((TVect, TVect) tuple) => new(tuple);
}
