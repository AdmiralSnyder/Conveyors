using ConveyorLibWeb.Shapes;
using InputLib;
using Microsoft.AspNetCore.Components.Web;
using UILib.Shapes;

namespace ConveyorBlazorServerNet7;

public class WebCanvasInputContext : InputContextBase
{
    private WebCanvasInfo _Canvas;
    public WebCanvasInfo Canvas 
    {
        get => _Canvas;
        set => Func.Setter(ref _Canvas, value, RegisterCanvas);
    }

    protected override bool IsShapeMatch(IShape shape, EventArgs e) => e is MouseEventArgs mea && shape is WebCanvasShape wcs && wcs.BackingShape.TryGetMouseAction(mea.Button switch
    {
        0 => MouseActions.LeftClick,
        1 => MouseActions.MiddleClick,
        2 => MouseActions.RightClick,
    }, out _)
        && wcs.BackingShape.ContainsPoint((mea.OffsetX, mea.OffsetY));
    protected override void InvokeShapeMouseDown(IShape shape, EventArgs e)
    {
        if (e is MouseEventArgs mea && shape is WebCanvasShape wcs && wcs.BackingShape.TryGetMouseAction(mea.Button switch
        {
            0 => MouseActions.LeftClick,
            1 => MouseActions.MiddleClick,
            2 => MouseActions.RightClick,
        }, out var action))
        {
            ((Action<IShape>)action).Invoke(shape);
        }
    }

    public override bool ShapesHaveEvents => false;

    private void RegisterCanvas(WebCanvasInfo oldCanvasInfo, WebCanvasInfo newCanvasInfo)
    {
        if (oldCanvasInfo is { Canvas: { } oldCanvas })
        {
            oldCanvas.MouseDown -= MouseDownEventHandler;
            //oldCanvas.MouseUp -= MouseUpEventHandler;
            oldCanvas.MouseMove -= MouseMoveEventHandler;
        }
        if (newCanvasInfo is { Canvas: { } newCanvas })
        {
            newCanvas.MouseDown += MouseDownEventHandler;
            //newCanvas.MouseUp += MouseUpEventHandler;
            newCanvas.MouseMove += MouseMoveEventHandler;
        }
    }

    private void MouseDownEventHandler(EventArgs args) => HandleMouseDown(null, args);
    private void MouseMoveEventHandler(EventArgs args) => HandleMouseMove(null, args);

    public override void AddTempShape(IShape shape) => Canvas.AddToCanvasTemporary(shape);

    public override void RemoveTempShape(IShape shape) => Canvas.RemoveFromCanvas(shape);
    public override Point GetPoint(EventArgs args)
        => args is MouseEventArgs mea
        ? GetPoint((mea.OffsetX, mea.OffsetY)) 
        : throw new NotImplementedException("unknown event type");

    public override Point GetPoint(Vector data) => data;

    public override bool IsMiddleClick(EventArgs args) => args is MouseEventArgs mea && mea.Button == 1;

    public override bool IsRightClick(EventArgs args) => args is MouseEventArgs mea && mea.Button == 2;

    protected override void HandleMouseDownVirtual(EventArgs e)
    {
        base.HandleMouseDownVirtual(e);
        if (e is MouseEventArgs mea)
        {
            if (mea.Button == 0)
            {
                DoLeftMouseButtonClicked(new(GetPoint(mea)));
            }
            else if (mea.Button == 2)
            {
                DoAbort();
            }
        }
    }



    
    
    public override bool IsShiftPressed() => false; // TODO


    public override void SetCursor(InputCursors cursor)
    {
        OnCursorChanged?.Invoke(null, cursor);
    }

    public event EventHandler<InputCursors> OnCursorChanged;

    protected override bool HandleMouseDownPanning(EventArgs e) => false;

    protected override bool HandleMouseMovePanning(EventArgs e) => false;

    protected override bool HandleMouseUpPanning(EventArgs e) => false;
}
