using InputLib.Inputters;

namespace InputLib;

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
        InputContext.UserNotes = "";
    }
}
