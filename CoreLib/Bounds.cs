namespace PointDef;

public struct Bounds<TVect>
{
    public Bounds(TVect location, TVect size) => (Location, Size) = (location, size);

    public TVect Location { get; set; }
    public TVect Size { get; set; }
}


