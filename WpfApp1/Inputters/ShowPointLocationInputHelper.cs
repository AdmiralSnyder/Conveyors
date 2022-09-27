using System.Windows.Input;
using System.Windows.Shapes;
using WpfLib;

namespace ConveyorApp.Inputters;

public class ShowPointerLocationInputHelper : Inputter<ShowPointerLocationInputHelper, Point, CanvasInputContext>
{

    protected override void AttachEvents() => Context.MouseMovedInCanvas += Context_MouseMovedInCanvas;
    protected override void DetachEvents() => Context.MouseMovedInCanvas -= Context_MouseMovedInCanvas;

    protected override void CleanupVirtual()
    {
        base.CleanupVirtual();
        Context.Canvas.Children.Remove(_TmpEllipse);
    }

    private Ellipse _TmpEllipse;

    private Ellipse TmpEllipse
    {
        get
        {
            if (_TmpEllipse is null)
            {
                _TmpEllipse = Context.MainWindow.ShapeProvider.CreateTempPoint(default);
                Context.Canvas.Children.Add(_TmpEllipse);
            }
            return _TmpEllipse;
        }
    }

    private void Context_MouseMovedInCanvas(object sender, MouseEventArgs e)
    {
        var point = Context.GetSnappedCanvasPoint(e);
        TmpEllipse.SetCenterLocation(point);
    }
}