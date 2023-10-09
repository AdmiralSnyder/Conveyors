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

    //public InputEntry<InitialInputState, Pair<InitialInputState, TNext>> Then2<TNext>(Func<InitialInputState, TNext> thenFunc, string? name = null)
    //{
    //    Func<InitialInputState, Task<InputResult<TNext>>> theTaskFunc =
    //        new(
    //            (init) => new Task<InputResult<TNext>>(
    //            state =>
    //            {
    //                var (_init, _thenFunc) = ((InitialInputState, Func<InitialInputState, TNext>))state;
    //                return _thenFunc(_init);
    //                //(new Pair<InitialInputState, TNext>()
    //                //{
    //                //    Previous = _init,
    //                //    Last = _thenFunc(_init)
    //                //});
    //            }, (init, thenFunc)));


    //    InputManager.AddStage(theTaskFunc, name);
    //    InputEntry<InitialInputState, Pair<InitialInputState, TNext>> newState = new() { InputManager = InputManager };
    //    return newState;
    //}

    public InputEntry<InitialInputState, Pair<InitialInputState, TNext>> Then<TNext>(Func<InitialInputState, TNext> thenFunc, string? name = null)
    {
        Func<InitialInputState, Task<InputResult<TNext>>> theTaskFunc =
            new(
                (init) => new Task<InputResult<TNext>>(
                state =>
                {
                    var (_init, _thenFunc) = ((InitialInputState, Func<InitialInputState, TNext>))state;
                    return _thenFunc(_init);
                    
                }, (init, thenFunc)));


        InputManager.AddStage(theTaskFunc, name);
        InputEntry<InitialInputState, Pair<InitialInputState, TNext>> newState = new() { InputManager = InputManager };
        return newState;
    }

    private async Task<InputResult<Pair<InitialInputState, TNext>>> SomeTask<TNext>()
    {
        await Task.Delay(1000);
        return new InputResult<Pair<InitialInputState, TNext>>(); // this also isn't
    }

}
