using ConveyorApp.Inputters;
using CoreLib;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConveyorApp;

public abstract class InputStage
{
    public virtual async Task Invoke() { await Task.Yield(); }
    public abstract bool IsSuccess();
}

public abstract class InputStage<TOutput> : InputStage
{
    public InputResult<TOutput> Output { get; set; }
}

public class InputStage<TInput, TOutput> : InputStage<TOutput>
{
    public TInput Input { get; set; }
    
    public Func<TInput, Task<InputResult<TOutput>>> StageFunc { get; set; }

    public override async Task Invoke() => Output = await StageFunc(Input);

    public override bool IsSuccess() => Output.Success;
}

public class InputEntryBase
{ 
    public InputManager InputManager { get; set; }
}

public class BlankInputEntry : InputEntryBase
{
    public BlankInputEntry(InputManager manager) => InputManager = manager;
    public InputEntry<InitialInputState, T> Then<T>(Func<InitialInputState, Task<InputResult<T>>> thenFunc)
    {
        InputManager.AddStage(thenFunc);
        InputEntry<InitialInputState, T> newState = new () { InputManager = InputManager };
        return newState;
    }
}

public class With<TIn, TOut>
{
    public TIn Input { get; set; }
    public TOut Output { get; set; }
}

public static class WithFunc
{
    public static (T1 Item1, T2 Item2) Flatten<T1, T2>(this With<T1, T2> with) => (with.Input, with.Output);
    public static bool Flatten<T1, T2>(this With<T1, T2> with, out (T1 Item1, T2 Item2) flattened)
    {
        flattened = (with.Input, with.Output);
        return true;
    }

    public static (T1 Item1, T2 Item2, T3 Item3) Flatten2<T1, T2, T3>(this With<With<T1, T2>, T3> with) => (with.Input.Input, with.Input.Output, with.Output);

    public static bool Flatten2<T1, T2, T3>(this With<With<T1, T2>, T3> with, out (T1 Item1, T2 Item2, T3 Item3) flattened)
    {
        flattened = (with.Input.Input, with.Input.Output, with.Output);
        return true;
    }
}

public class InitialInputState : InputState
{ }
public class InputState { }

public class InputEntry<TIn, TOut> : InputEntryBase
{
    public InputEntry<TOut, With<TOut, TNext>> Then<TNext>(Func<TOut, Task<InputResult<TNext>>> thenFunc)
    {
        InputManager.AddStage(thenFunc);
        InputEntry<TOut, With<TOut, TNext>> newState = new() { InputManager = InputManager };
        return newState;
    }

    internal async Task<InputResult<TOut>> Do()
    {
        var result = await InputManager.Run<TOut>();
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

    public void AddStage<TInput, T>(Func<TInput, Task<InputResult<T>>> stageFunc)
    {
        Stages.Enqueue(new InputStage<TInput, T>() { StageFunc = stageFunc });
    }

    internal async Task<InputResult<T>> Run<T>()
    {
        InputStage lastStage = null;
        while(true)
        {
            if (Stages.Any())
            {
                var stage = Stages.Dequeue();
                await stage.Invoke();
                if (stage.IsSuccess())
                {
                    DoneStages.Enqueue(stage);
                    lastStage = stage;
                }
                // TODO else retry depending on strategy
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

    protected void DoAbort() => Abort?.Invoke(this, EventArgs.Empty);

    public event EventHandler<EventArgs<Point>> LeftMouseButtonClicked;

    protected void DoLeftMouseButtonClicked(Point point) => LeftMouseButtonClicked?.Invoke(this, new(point));

    internal void HandleMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (HandleMouseUpPanning(e)) return;
    }

    protected abstract bool HandleMouseUpPanning(MouseButtonEventArgs e);

}
