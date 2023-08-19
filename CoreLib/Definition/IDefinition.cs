namespace CoreLib.Definition;

public interface IDefinition<TSource>
{
    TSource? GetSource();
    void ApplySource(TSource source);
}
