namespace InputLib.Inputters;

public class AbortingInputter<TThis, TResult> : Inputter<TThis, TResult>
    where TThis : AbortingInputter<TThis, TResult>, new()
{
    protected override void AttachEvents()
    {
        base.AttachEvents();
        Context.Abort += Context_Abort;
    }

    protected override void DetachEvents()
    {
        base.DetachEvents();
        Context.Abort -= Context_Abort;
    }

    async void Context_Abort(object? sender, EventArgs e) => Abort();
}
