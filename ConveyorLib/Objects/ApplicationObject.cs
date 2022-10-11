namespace ConveyorLib.Objects;

public abstract class ApplicationObject<TApplication> : ISelectObject, IRefreshable, IAppObject<TApplication>
    where TApplication : IApplication
{
    public virtual string Text => Name;
    public string Name { get; set; }
    public virtual Vector[] GetSelectionBoundsPoints() => null;
    public virtual ISelectObject? SelectionParent => null;
}
