namespace CoreLib;

public readonly struct Option<T> 
{
    public bool Success { get; init; }
    public T Value { get; init; }

    public static implicit operator Option<T>(T value) => new() { Value = value, Success = true };
    public static Option<T> Fail = new() { Value = default, Success = false };
}