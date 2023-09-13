namespace ConveyorApp.Inputters.Helpers;

public class ShowUserNotesInputHelper : Inputter<ShowUserNotesInputHelper, InputContextBase>
{
    public static ShowUserNotesInputHelper Create(InputContextBase context, string userNotes)
    {
        context.UserNotes = userNotes;
        return Create(context);
    }

    protected override void CleanupVirtual()
    {
        base.CleanupVirtual();
        Context.UserNotes = "";
    }
}
