namespace CoreLib;

public class Pair<TFirst, TSecond>
{
    public TFirst Previous { get; set; }
    public TSecond Last { get; set; }
}