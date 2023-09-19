using InputLib;

namespace ConveyorApp.Inputters.Helpers;

public class ShowPickedSelectableInputHelper : Inputter<ShowPickedSelectableInputHelper, WpfCanvasInputContext>
{
    private ObjectHighlighter? ObjectHighlighter { get; set; }
    protected override void CleanupVirtual()
    {
        base.CleanupVirtual();
        ObjectHighlighter!.SelectObject = null;
        ObjectHighlighter = null;
    }

    public static ShowPickedSelectableInputHelper Create(WpfCanvasInputContext context, ISelectObject? selectObject)
    {
        var result = Create(context);
        result.ObjectHighlighter = UIHelpers.CreateObjectHighlighter(context.Canvas, selectObject);
        return result;
    }
}
