using ConveyorInputLib.Helpers;

namespace ConveyorApp.Inputters.Helpers;

public class WpfCanvasInputHelpers : CanvasInputHelpers
{


public ShowPickedSelectableInputHelper ShowPickedSelectable(ISelectObject selectable)
=> ShowPickedSelectableInputHelper.Create((WpfCanvasInputContext)Context, selectable);

}
