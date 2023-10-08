using CoreLib;
using System;
using System.Threading.Tasks;
using UILib.Shapes;

namespace InputLib.Inputters;

public abstract class Inputter : IInputter
{
    public virtual void HandleMouseDown(object sender, EventArgs e) { }

    public virtual void HandleMouseMove(object sender, EventArgs e) { }

    public abstract void Start();

    public abstract void RunAsync();

    public abstract void Abort();

    public virtual IEnumerable<IShape> GetMouseDownShapes() => [];
}

public abstract class InputterBase<TThis, TTask> : Inputter
    where TThis : InputterBase<TThis, TTask>, new()
{
    private InputContextBase? _Context;

    public InputContextBase InputContext
    {
        get => _Context ?? throw new NullReferenceException("Context darf nicht null sein");
        private set => Func.Setter(ref _Context, value, ContextAssigned);
    }

    protected virtual void ContextAssigned() { }

    public static TThis Create(InputContextBase context) => new() { InputContext = context };

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

    protected virtual void CleanupVirtual() => InputContext.ClearInputter(this);

    protected virtual void AbortVirtual() { }

    protected virtual void AttachEvents() { }

    protected virtual void DetachEvents() { }

    private Inputter[]? SubInputters { get; set; }

    public static TTask StartInput(InputContextBase context, params Inputter[] subInputters)
    {
        var inputter = Create(context);
        return inputter.RunWithSubInputters(subInputters);
    }

    private TTask RunWithSubInputters(params Inputter[] subInputters)
    {
        SubInputters = subInputters;
        return StartAsync();
    }

    public TTask Run(InputContextBase context, params Inputter[] subInputters)
    {
        InputContext = context;
        return RunWithSubInputters(subInputters);
    }
}

public abstract class Inputter<TThis> : InputterBase<TThis, Task>
    where TThis : Inputter<TThis>, new()
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

public abstract class Inputter<TThis, TResult> : InputterBase<TThis, Task<InputResult<TResult>>>
    where TThis : Inputter<TThis, TResult>, new()
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

    private static async Task<(bool Success, TResult Result)> StartedSuccessful(InputContextBase inputContext) 
        => ((await StartInput(inputContext)).IsSuccess(out var result), result);

    public static async Task<TResult> StartInputOnce(InputContextBase inputContext)
    {
        // TODO what is better?
        //if((await StartInput(inputContext)).IsSuccess(out var result))

        var b = await StartedSuccessful(inputContext);
        if (b.Success)
        {
            return b.Result;
        }
        else
        {
            return await Task.FromCanceled<TResult>(CancellationToken.None);
        }
    }

    public static async IAsyncEnumerable<TResult> StartInputContinuous(InputContextBase inputContext)
    {
        while ((await StartInput(inputContext)).IsSuccess(out var result))
        {
            yield return result;
        }
    }
}

public abstract class Inputter<TThis, TResult, THelpers> : Inputter<TThis, TResult>
    where TThis : Inputter<TThis, TResult, THelpers>, new()
    where THelpers : InputHelpers, new()
{
    public THelpers Helpers { get; private set; }
    protected override void ContextAssigned() => Helpers = new() { Context = InputContext };
}
