using CoreLib;

namespace InputLib;

public class InputEntry<TIn, TOut> : InputEntryBase
{
    public InputEntry<TOut, Pair<TOut, TNext>> Then<TNext>(Func<TOut, Task<InputResult<TNext>>> thenFunc, string? name = null)
    {
        InputManager.AddStage(thenFunc, name);
        InputEntry<TOut, Pair<TOut, TNext>> newState = new() { InputManager = InputManager };
        return newState;
    }

    public InputEntry<TOut, Pair<TOut, TNext>> Then<TNext>(Func<TOut, InputResult<TNext>> thenFunc, string? name = null)
    {
        Func<TOut, Task<InputResult<TNext>>> theTaskFunc = new((tOut) => new Task<InputResult<TNext>>(() => thenFunc(tOut)));
        InputManager.AddStage(theTaskFunc, name);
        InputEntry<TOut, Pair<TOut, TNext>> newState = new() { InputManager = InputManager };
        return newState;
    }

    public InputEntry<TOut, Pair<TOut, TNext>> Then2<TNext>(Func<TOut, InputResult<TNext>> thenFunc, string? name = null)
    {
        Func<TOut, Task<InputResult<TNext>>> theTaskFunc = new((tOut) => new Task<InputResult<TNext>>(() => thenFunc(tOut)));
        InputManager.AddStage(theTaskFunc, name);
        InputEntry<TOut, Pair<TOut, TNext>> newState = new() { InputManager = InputManager };
        return newState;
    }

    public InputEntry<TOut, TOut> Then(Func<TOut, InputResult<TOut>> thenFunc, string? name = null)
    {
        Func<TOut, Task<InputResult<TOut>>> theTaskFunc = new((tOut) => new Task<InputResult<TOut>>(() => thenFunc(tOut)));
        InputManager.AddStage(theTaskFunc, name);
        InputEntry<TOut, TOut> newState = new() { InputManager = InputManager };
        return newState;
    }

    public InputEntry<TOut, TOut> Then(Action<TOut> thenAction, string? name = null)
    {
        Func<TOut, Task<InputResult<TOut>>> theTaskFunc = new((tOut) =>
        {
            thenAction(tOut);
            return new Task<InputResult<TOut>>(() => new InputResult<TOut>(tOut));
        });
        InputManager.AddStage(theTaskFunc, name);
        InputEntry<TOut, TOut> newState = new() { InputManager = InputManager };
        return newState;
    }

    public async Task<InputResult<TResult>> Do<TResult>(Func<TOut, Task<InputResult<TResult>>> doFunc)
    {
        InputManager.AddStage(doFunc);
        var result = await InputManager.Run<TResult>();
        return result;
    }
}
