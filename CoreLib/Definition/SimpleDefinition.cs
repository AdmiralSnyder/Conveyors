namespace CoreLib.Definition;

public class SimpleDefinition<TSource> : IDefinition<TSource>
{
    public TSource? Source { get; set; }
    public void ApplySource(TSource source) => Source = source;

    public TSource? GetSource() => Source;
}
