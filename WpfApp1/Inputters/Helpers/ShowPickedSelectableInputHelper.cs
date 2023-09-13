﻿namespace ConveyorApp.Inputters.Helpers;

public class ShowPickedSelectableInputHelper : Inputter<ShowPickedSelectableInputHelper, CanvasInputContext>
{
    private CanvasObjectHighlighter? CanvasObjectHighlighter { get; set; }
    protected override void CleanupVirtual()
    {
        base.CleanupVirtual();
        CanvasObjectHighlighter!.SelectObject = null;
        CanvasObjectHighlighter = null;
    }

    //private ISelectObject SelectObject { get; set; }

    //public ISelectObject SelectObject 
    //{
    //    get => _SelectObject; 
    //    set => Func.Setters(ref _SelectObject, value); 
    //}

    public static ShowPickedSelectableInputHelper Create(CanvasInputContext context, ISelectObject selectObject)
    {
        var result = Create(context);
        result.CanvasObjectHighlighter = CanvasObjectHighlighter.Create(context.Canvas, selectObject);
        return result;
    }
}
