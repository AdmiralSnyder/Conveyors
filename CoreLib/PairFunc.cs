using System;

namespace CoreLib;

public static class PairFunc
{
    public static (T1 Item1, T2 Item2) Flatten<T1, T2>(this Pair<T1, T2> with) => (with.Previous, with.Last);
    public static bool Flatten<T1, T2>(this Pair<T1, T2> with, out (T1 Item1, T2 Item2) flattened)
    {
        flattened = (with.Previous, with.Last);
        return true;
    }

    public static (T1 Item1, T2 Item2, T3 Item3) Flatten2<T1, T2, T3>(this Pair<Pair<T1, T2>, T3> with) => (with.Previous.Previous, with.Previous.Last, with.Last);

    public static bool Flatten2<T1, T2, T3>(this Pair<Pair<T1, T2>, T3> with, out (T1 Item1, T2 Item2, T3 Item3) flattened)
    {
        flattened = (with.Previous.Previous, with.Previous.Last, with.Last);
        return true;
    }

    public static (T1R Item, T2) Map<T1, T2, T1R>(this (T1, T2) pair, Func<T1, T1R> mapper) => (mapper(pair.Item1), pair.Item2); 
    public static (T1R Item, T2R) Map<T1, T2, T1R, T2R>(this (T1, T2) pair, Func<T1, T1R> mapper1, Func<T2, T2R> mapper2) => (mapper1(pair.Item1), mapper2(pair.Item2));
    public static (T1R Item, T2, T3) Map<T1, T2, T3, T1R>(this (T1, T2, T3) pair, Func<T1, T1R> mapper) => (mapper(pair.Item1), pair.Item2, pair.Item3);
    public static (T1R Item, T2R, T3) Map<T1, T2, T3, T1R, T2R>(this (T1, T2, T3) pair, Func<T1, T1R> mapper1, Func<T2, T2R> mapper2) => (mapper1(pair.Item1), mapper2(pair.Item2), pair.Item3);
    public static (T1R Item, T2R, T3R) Map<T1, T2, T3, T1R, T2R, T3R>(this (T1, T2, T3) pair, Func<T1, T1R> mapper1, Func<T2, T2R> mapper2, Func<T3, T3R> mapper3) => (mapper1(pair.Item1), mapper2(pair.Item2), mapper3(pair.Item3));

}
