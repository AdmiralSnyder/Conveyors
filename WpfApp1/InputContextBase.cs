using CoreLib;
using System;
using System.Windows.Input;

namespace ConveyorApp;

public abstract class InputContextBase
{
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
