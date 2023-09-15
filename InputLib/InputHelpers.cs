namespace InputLib;

//public class InputHelpers { }

public class InputHelpers
    //where TContext : InputContextBase
{
    public InputContextBase Context { get; set; }

    public ShowUserNotesInputHelper ShowUserNotes(string notes)
        => ShowUserNotesInputHelper.Create(Context, notes);
}
