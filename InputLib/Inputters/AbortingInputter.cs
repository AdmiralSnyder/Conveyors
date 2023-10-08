namespace InputLib.Inputters;

// TODO I hate the diamond problem!!
public class AbortingInputter<TThis, TResult> : Inputter<TThis, TResult>
    where TThis : AbortingInputter<TThis, TResult>, new()
{
    protected override void AttachEvents()
    {
        base.AttachEvents();
        InputContext.Abort += Context_Abort;
    }

    protected override void DetachEvents()
    {
        base.DetachEvents();
        InputContext.Abort -= Context_Abort;
    }

    async void Context_Abort(object? sender, EventArgs e) => Abort();
}

public class AbortingInputter<TThis, TResult, THelpers> : Inputter<TThis, TResult, THelpers>
    where TThis : AbortingInputter<TThis, TResult, THelpers>, new()
    where THelpers : InputHelpers, new()
{
    protected override void AttachEvents()
    {
        base.AttachEvents();
        InputContext.Abort += Context_Abort;
    }

    protected override void DetachEvents()
    {
        base.DetachEvents();
        InputContext.Abort -= Context_Abort;
    }

    async void Context_Abort(object? sender, EventArgs e) => Abort();
}