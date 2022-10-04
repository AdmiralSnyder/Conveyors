using CoreLib;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConveyorApp.Inputters;

public abstract class Inputter
{
    public virtual void HandleMouseDown(object sender, MouseButtonEventArgs e) { }

    public virtual void HandleMouseMove(object sender, MouseEventArgs e) { }

    public abstract void Start();
    public abstract void RunAsync();

    public abstract void Abort();
}

public abstract class InputterBase<TThis, TContext, TTask> : Inputter
    where TThis : InputterBase<TThis, TContext, TTask>, new()
    where TContext : InputContextBase
{
    private TContext _Context;

    public TContext Context 
    {
        get => _Context;
        private set => Func.Setter(ref _Context, value, ContextAssigned);
    }

    protected virtual void ContextAssigned() { }
    
    public static TThis Create(TContext context) => new() { Context = context };

    public override void Start() => AttachEvents();

    protected abstract TTask StartAsyncVirtual();

    public override void RunAsync() => StartAsync();

    public virtual TTask StartAsync()
    {
        if (SubInputters is { })
        {
            foreach (var si in SubInputters)
            {
                si.RunAsync();
            }
        }
        return StartAsyncVirtual();
    }

    public virtual void Complete() => Cleanup();

    public override void Abort()
    {
        Cleanup();
        AbortVirtual();
    }

    public void Cleanup()
    {
        if (SubInputters is { })
        {
            foreach (var si in SubInputters)
            {
                si.Abort();
            }
        }
        DetachEvents();
        CleanupVirtual();
    }

    protected virtual void CleanupVirtual() { }

    protected virtual void AbortVirtual() { }

    protected virtual void AttachEvents() { }

    protected virtual void DetachEvents() { }

    private Inputter[] SubInputters { get; set; }

    public static TTask StartInput(TContext context, params Inputter[] subInputters)
    {
        var inputter = Create(context);
        inputter.SubInputters = subInputters;
        return inputter.StartAsync();
    }

}

public abstract class Inputter<TThis, TContext> : InputterBase<TThis, TContext, Task>
    where TThis : Inputter<TThis, TContext>, new()
    where TContext : InputContextBase
{
    protected TaskCompletionSource TaskCompletionSource { get; set; }

    public override void Complete()
    {
        base.Complete();
        TaskCompletionSource.SetResult();
    }

    protected override Task StartAsyncVirtual()
    {
        Start();
        TaskCompletionSource = new();
        return TaskCompletionSource.Task;
    }
}

public abstract class Inputter<TThis, TResult, TContext> : InputterBase<TThis, TContext, Task<InputResult<TResult>>>
    where TThis : Inputter<TThis, TResult, TContext>, new()
    where TContext : InputContextBase
{
    protected TaskCompletionSource<InputResult<TResult>> TaskCompletionSource { get; set; }


    public TResult Result { get; protected set; }

    public void Complete(TResult result)
    {
        Result = result;
        Complete();
    }

    public override void Abort()
    {
        TaskCompletionSource.SetResult(InputResult.Failure);

        base.Abort();
    }

    public override void Complete()
    {
        base.Complete();
        TaskCompletionSource.SetResult(Result);
    }

    protected override Task<InputResult<TResult>> StartAsyncVirtual()
    {
        Start();
        TaskCompletionSource = new();
        return TaskCompletionSource.Task;
    }
}

public abstract class Inputter<TThis, TResult, TContext, THelpers> : Inputter<TThis, TResult, TContext>
    where TThis : Inputter<TThis, TResult, TContext, THelpers>, new()
    where TContext : InputContextBase
    where THelpers : InputHelpers<TContext>, new()
{
    public THelpers Helpers { get; private set; }
    protected override void ContextAssigned() => Helpers = new() { Context = Context };
}

public abstract class StatefulInputter<TThis, TResult, TInputState, TContext> : Inputter<TThis, TResult, TContext>
    where TThis : StatefulInputter<TThis, TResult, TInputState, TContext>, new()
    where TInputState : struct, Enum
    where TContext : InputContextBase
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

    protected virtual void InputStateChanged(TInputState oldValue, TInputState newValue)
    {
        InputStateChanged(newValue);
    }

    protected virtual void InputStateChanged(TInputState newValue)
    { }
}
