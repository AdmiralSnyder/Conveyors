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

    public async Task<InputResult<TResult>> Do<TResult>(Func<TOut, Task<InputResult<TResult>>> doFunc)
    {
        InputManager.AddStage(doFunc);
        var result = await InputManager.Run<TResult>();
        return result;
    }
}
