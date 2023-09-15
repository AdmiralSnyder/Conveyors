using CoreLib;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UILib;
using UILib.Shapes;

namespace InputLib;

public abstract class InputContextBase
{
    public abstract bool ShapesHaveEvents { get; }

    public abstract void AddTempShape(IShape shape);
    public abstract void RemoveTempShape(IShape shape);

    public abstract Point GetPoint(EventArgs args);
    public abstract Point GetPoint(Point data);
    public abstract bool IsShiftPressed();
    public abstract bool IsRightClick(EventArgs args);
    public abstract bool IsMiddleClick(EventArgs args);

    public InputManager InputManager { get; set; }

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

    public event EventHandler Abort;

    public event EventHandler<EventArgs<(ISelectObject, Point)>> ObjectPicked;

    protected void DoObjectPicked(ISelectObject pickedObject, Point point) => ObjectPicked?.Invoke(this, new((pickedObject, point)));

    protected void DoAbort() => Abort?.Invoke(this, EventArgs.Empty);

    public event EventHandler<EventArgs<Point>> LeftMouseButtonClicked;

    protected void DoLeftMouseButtonClicked(Point point) => LeftMouseButtonClicked?.Invoke(this, new(point));

    public Inputter? CurrentInputter { get; set; }

    //public MainWindow MainWindow { get; set; }

    public abstract void SetCursor(InputCursors cursor);

    protected void HandleMouseDown(object sender, EventArgs e)
    {
        if (!this.ShapesHaveEvents)
        {
            if (HandleShapeMouseDown(e)) return;
        }

        if (HandleMouseDownPanning(e)) return;

        CurrentInputter?.HandleMouseDown(sender, e);

        HandleMouseDownVirtual(e);
    }

    private bool HandleShapeMouseDown(EventArgs e)
    {
        // TODO this is slow and should be improved.
        if (CurrentInputter?.GetMouseDownShapes() is { } shapes)
        {
            foreach (var shape in shapes)
            {
                if (IsShapeMatch(shape, e))
                {
                    InvokeShapeMouseDown(shape, e);
                    return true;
                }
            }
        }

        return false;
    }

    protected virtual bool IsShapeMatch(IShape shape, EventArgs e) => false;
    protected virtual void InvokeShapeMouseDown(IShape shape, EventArgs e) { }

    protected void HandleMouseMove(object sender, EventArgs e)
    {
        if (HandleMouseMovePanning(e)) return;

        CurrentInputter?.HandleMouseMove(sender, e);
        MouseMovedInCanvas?.Invoke(sender, e);
        Notify();
    }

    protected void HandleMouseUp(object sender, EventArgs e)
    {
        if (HandleMouseUpPanning(e)) return;
    }

    protected virtual void HandleMouseDownVirtual(EventArgs e) { }
    protected abstract bool HandleMouseDownPanning(EventArgs e);
    protected abstract bool HandleMouseMovePanning(EventArgs e);

    public event EventHandler MouseMovedInCanvas;

    //public event MouseButtonEventHandler LeftMouseButtonClicked;
    //public event MouseButtonEventHandler RightMouseButtonClicked;
    //public event MouseButtonEventHandler MouseWheelClicked;

    protected abstract bool HandleMouseUpPanning(EventArgs e);

    public void ClearInputter(IInputter inputter)
    {
        if (CurrentInputter == inputter)
        {
            CurrentInputter = null;
        }
    }

    internal void Notify() => OnNotify?.Invoke();

    public event Action OnNotify;
}

public enum InputCursors
{
    Arrow,
    Normal = Arrow,
    Hand,
    Cross,
}