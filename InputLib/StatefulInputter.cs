namespace InputLib;

public abstract class StatefulInputter<TThis, TResult, TInputState> : Inputter<TThis, TResult>
    where TThis : StatefulInputter<TThis, TResult, TInputState>, new()
    where TInputState : struct, Enum
{
    private TInputState _InputState;
    protected TInputState InputState
    {
        get => _InputState;
        set
        {
            if (!value.Equals(_InputState))
            {
                var oldValue = _InputState;
                _InputState = value;
                InputStateChanged(oldValue, value);
            }
        }
    }

    protected virtual void InputStateChanged(TInputState oldValue, TInputState newValue) => InputStateChanged(newValue);

    protected virtual void InputStateChanged(TInputState newValue) => Context.Notify();
}
