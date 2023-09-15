using CoreLib;

namespace InputLib;

public static class InputStatePairFunc
{
    public static (T1 Item1, T2 Item2) Flatten<T1, T2>(this Pair<Pair<InitialInputState, T1>, T2> with) => (with.First.Second, with.Second);

    public static (T1 Item1, T2 Item2, T3 Item3) Flatten<T1, T2, T3>(this Pair<Pair<Pair<InitialInputState, T1>, T2>, T3> with) => (with.First.First.Second, with.First.Second, with.Second);
}
