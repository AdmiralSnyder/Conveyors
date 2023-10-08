using CoreLib;

namespace InputLib;

public class BlankInputEntry : InputEntryBase
{
    public BlankInputEntry(InputManager manager) => InputManager = manager;
    public InputEntry<InitialInputState, Pair<InitialInputState, TNext>> Then<TNext>(Func<InitialInputState, Task<InputResult<TNext>>> thenFunc, string? name = null)
    {
        InputManager.AddStage(thenFunc, name);
        InputEntry<InitialInputState, Pair<InitialInputState, TNext>> newState = new() { InputManager = InputManager };
        return newState;
    }

    public InputEntry<InitialInputState, Pair<InitialInputState, TNext>> Then<TNext>(Func<InitialInputState, TNext> thenFunc, string? name = null)
    {
        Func<InitialInputState, Task<InputResult<Pair<InitialInputState, TNext>>>> theTaskFunc =
            new(
                (init) => new Task<InputResult<Pair<InitialInputState, TNext>>>(
                state =>
                {
                    var (_init, _thenFunc) = ((InitialInputState, Func<InitialInputState, TNext>))state;
                    return (new Pair<InitialInputState, TNext>()
                    {
                        Previous = _init,
                        Last = _thenFunc(_init)
                    });
                }, (init, thenFunc)));


        InputManager.AddStage<InitialInputState, Pair<InitialInputState, TNext>>(theTaskFunc, name);
        InputEntry<InitialInputState, Pair<InitialInputState, TNext>> newState = new() { InputManager = InputManager };
        return newState;
    }

    private object ThenFunc;


    private Task<InputResult<Pair<InitialInputState, TNext>>> TheTaskFunc<TNext>(InitialInputState init)
    {
        var thenFunc = (Func<InitialInputState, TNext>)ThenFunc;

        // this gets returned when awaited
        //return Task.FromResult<InputResult<Pair<InitialInputState, TNext>>>(default);
        // this is waiting forever
        //return SomeTask<TNext>();
        return new Task<InputResult<Pair<InitialInputState, TNext>>>(
                state =>
                {
                    Task.Delay(1000);
                    return new Pair<InitialInputState, TNext>(); // this also isn't
                    var input = ((InitialInputState _init, Func<InitialInputState, TNext> _thenFunc))state;
                    return (new Pair<InitialInputState, TNext>()
                    {
                        Previous = input._init,
                        Last = input._thenFunc(input._init)
                    });
                }, (init, ThenFunc));

    }

    private async Task<InputResult<Pair<InitialInputState, TNext>>> SomeTask<TNext>()
    {
        await Task.Delay(1000);
        return new InputResult<Pair<InitialInputState, TNext>>(); // this also isn't
    }

}
