namespace PointDef;

public struct Interval<T>
{
    public Interval(T min, T max) => (Min, Max) = (min, max);
    public Interval((T Min, T Max) tuple) => (Min, Max) = tuple;

    public T Min { get; set; }
    public T Max { get; set; }

    public static implicit operator Interval<T>((T, T) tuple) => new(tuple);
}


