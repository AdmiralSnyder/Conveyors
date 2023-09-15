using CoreLib;

namespace InputLib;

public class BlankInputEntry : InputEntryBase
{
    public BlankInputEntry(InputManager manager) => InputManager = manager;
    public InputEntry<InitialInputState, Pair<InitialInputState, T>> Then<T>(Func<InitialInputState, Task<InputResult<T>>> thenFunc, string? name = null)
    {
        InputManager.AddStage(thenFunc, name);
        InputEntry<InitialInputState, Pair<InitialInputState, T>> newState = new() { InputManager = InputManager };
        return newState;
    }
}
