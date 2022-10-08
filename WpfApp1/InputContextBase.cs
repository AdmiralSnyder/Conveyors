using ConveyorApp.Inputters;
using CoreLib;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UILib;

namespace ConveyorApp;

public class InputStage
{
    public virtual async Task Invoke(InputStage? input) => throw new NotImplementedException("Needs to be overridden");
    public virtual bool IsSuccess() => false;
    public InputStage() => Name = "IS" + Cnt++;
    public string? Name { get; set; }

    private static int Cnt = 0;

    public static readonly InputStage<InitialInputState> Initial = new() { Output = InitialInputState.Instance };

    public object? NextInput { get; set; }
}

public class InputStage<TOutput> : InputStage
{
    public InputResult<TOutput> Output { get; set; }

    //public string SimpleStageType => typeof(InputStage<TOutput>).FullName;
}

public class InputStage<TInput, TOutput> : InputStage<TOutput>
{
    public TInput Input { get; set; }
    
    public Func<TInput, Task<InputResult<TOutput>>> StageFunc { get; set; }

    public override async Task Invoke(InputStage? input)
    {
        if (input?.NextInput is TInput lastOutput)
        {
            Input = lastOutput;
        }
        var result = await StageFunc(Input);
        Output = result;
        if (result.IsSuccess(out var resResult))
        {
            NextInput = new Pair<TInput, TOutput>() { First = Input, Second = resResult };
        }
    }

    public override bool IsSuccess() => Output.Success;
}

public class InputEntryBase
{ 
    public InputManager InputManager { get; set; }
}

public class BlankInputEntry : InputEntryBase
{
    public BlankInputEntry(InputManager manager) => InputManager = manager;
    public InputEntry<InitialInputState, Pair<InitialInputState, T>> Then<T>(Func<InitialInputState, Task<InputResult<T>>> thenFunc, string? name = null)
    {
        InputManager.AddStage(thenFunc, name);
        InputEntry<InitialInputState, Pair<InitialInputState, T>> newState = new () { InputManager = InputManager };
        return newState;
    }
}

public class Pair<TFirst, TSecond>
{
    public TFirst First { get; set; }
    public TSecond Second { get; set; }
}

public static class PairFunc
{
    public static (T1 Item1, T2 Item2) Flatten<T1, T2>(this Pair<T1, T2> with) => (with.First, with.Second);
    public static bool Flatten<T1, T2>(this Pair<T1, T2> with, out (T1 Item1, T2 Item2) flattened)
    {
        flattened = (with.First, with.Second);
        return true;
    }

    public static (T1 Item1, T2 Item2) Flatten<T1, T2>(this Pair<Pair<InitialInputState, T1>, T2> with) => (with.First.Second, with.Second);

    public static (T1 Item1, T2 Item2, T3 Item3) Flatten<T1, T2, T3>(this Pair<Pair<Pair<InitialInputState, T1>, T2>, T3> with) => (with.First.First.Second, with.First.Second, with.Second);
    public static (T1 Item1, T2 Item2, T3 Item3) Flatten2<T1, T2, T3>(this Pair<Pair<T1, T2>, T3> with) => (with.First.First, with.First.Second, with.Second);

    public static bool Flatten2<T1, T2, T3>(this Pair<Pair<T1, T2>, T3> with, out (T1 Item1, T2 Item2, T3 Item3) flattened)
    {
        flattened = (with.First.First, with.First.Second, with.Second);
        return true;
    }
}

public class InitialInputState : InputState 
{
    public static readonly InitialInputState Instance = new();
}
public class InputState { }

public class InputEntry<TIn, TOut> : InputEntryBase
{
    public InputEntry<TOut, Pair<TOut, TNext>> Then<TNext>(Func<TOut, Task<InputResult<TNext>>> thenFunc, string? name = null)
    {
        InputManager.AddStage(thenFunc, name);
        InputEntry<TOut, Pair<TOut, TNext>> newState = new() { InputManager = InputManager };
        return newState;
    }

    internal async Task<InputResult<TResult>> Do<TResult>(Func<TOut, Task<InputResult<TResult>>> doFunc)
    {
        InputManager.AddStage(doFunc);
        var result = await InputManager.Run<TResult>();
        return result;
    }
}

public class InputManager
{
    private Queue<InputStage> Stages = new();

    private Queue<InputStage> DoneStages = new();

    internal static BlankInputEntry Blank()
    {
        var manager = new InputManager();
        var blankEntry = new BlankInputEntry(manager);
        return blankEntry;
    }

    public void AddStage<TInput, T>(Func<TInput, Task<InputResult<T>>> stageFunc, string? name = null)
    {
        Stages.Enqueue(new InputStage<TInput, T>() { StageFunc = stageFunc, Name = name });
    }

    internal async Task<InputResult<T>> Run<T>()
    {
        InputStage lastStage = InputStage.Initial;
        while(true)
        {
            if (Stages.Any())
            {
                var stage = Stages.Dequeue();
                await stage.Invoke(lastStage);
                if (stage.IsSuccess())
                {
                    DoneStages.Enqueue(stage);
                    lastStage = stage;
                }
                else
                {
                // TODO else retry depending on strategy
                    return InputResult.Failure;
                }
            }
            else break;
        }

        if (lastStage is InputStage<T> lastStageT)
        {
            return lastStageT.Output;
        }
        else
        {
            return InputResult.Failure;
        }
    }
}

public abstract class InputContextBase
{
    public InputManager InputManager { get; set; }

    public MainWindow MainWindow { get; set; }

    public abstract void SetCursor(Cursor cursor);

    private string? _UserNotes;
    public string UserNotes
    {
        get => _UserNotes;
        set
        {
            _UserNotes = value;
            UserNotesChanged();
        }
    }

    public virtual void CaptureMouse() { }
    public virtual void ReleaseMouseCapture() { }

    protected virtual void UserNotesChanged() { }

    internal void HandleMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (HandleMouseDownPanning(e)) return;

        if (MainWindow.CurrentInputter is { } ci)
        {
            ci.HandleMouseDown(sender, e);
        }

        HandleMouseDownVirtual(e);
    }

    protected virtual void HandleMouseDownVirtual(MouseButtonEventArgs e) { }

    protected abstract bool HandleMouseDownPanning(MouseButtonEventArgs e);

    protected abstract bool HandleMouseMovePanning(MouseEventArgs e);

    internal void HandleMouseMove(object sender, MouseEventArgs e)
    {
        if (HandleMouseMovePanning(e)) return;

        MouseMovedInCanvas?.Invoke(sender, e);

        if (MainWindow.CurrentInputter is { } ci)
        {
            ci.HandleMouseMove(sender, e);
        }
    }

    public event MouseEventHandler MouseMovedInCanvas;

    //public event MouseButtonEventHandler LeftMouseButtonClicked;
    //public event MouseButtonEventHandler RightMouseButtonClicked;
    //public event MouseButtonEventHandler MouseWheelClicked;

    public event EventHandler Abort;

    public event EventHandler<EventArgs<(ISelectObject, Point)>> ObjectPicked;

    protected void DoObjectPicked(ISelectObject pickedObject, Point point)
    {
        ObjectPicked?.Invoke(this, new((pickedObject, point)));
    }

    protected void DoAbort() => Abort?.Invoke(this, EventArgs.Empty);

    public event EventHandler<EventArgs<Point>> LeftMouseButtonClicked;

    protected void DoLeftMouseButtonClicked(Point point) => LeftMouseButtonClicked?.Invoke(this, new(point));

    internal void HandleMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (HandleMouseUpPanning(e)) return;
    }

    protected abstract bool HandleMouseUpPanning(MouseButtonEventArgs e);

}
