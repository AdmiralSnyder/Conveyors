using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputLib;

public static class InputResult
{
    public static FailureInputResult Failure { get; } = FailureInputResult.Instance;

    public static InputResult<T> Success<T>(T result) => new(result);

    public static Task<InputResult<T>> SuccessTask<T>(T result) => Task.FromResult(Success(result));
}

public class FailureInputResult
{
    public static FailureInputResult Instance { get; } = new();
    private FailureInputResult() { }
}

public class InputResult<T>
{
    private static readonly InputResult<T> Failure = new();

    internal InputResult() { }
    internal InputResult(T result)
    {
        Success = true;
        Result = result;
    }

    public bool Success { get; private set; }
    public T Result { get; private set; } = default!;

    public bool IsSuccess(out T result)
    {
        result = Result;
        return Success;
    }

    public static implicit operator InputResult<T>(FailureInputResult _) => Failure;
    public static implicit operator InputResult<T>(T result) => new(result);
}
