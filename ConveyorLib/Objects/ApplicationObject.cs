namespace ConveyorLib.Objects;

public abstract class ApplicationObject<TApplication> : ISelectObject, IRefreshable, IAppObject<TApplication>
    where TApplication : IApplication
{
    public string ID { get; set; } = Guid.NewGuid().ToString();
    public virtual string Text => Name;
    public string Name { get; set; }
    public virtual Vector[] GetSelectionBoundsPoints() => null;

    public virtual ISelectObject? SelectionParent => null;
    public abstract bool IsSelectionMatch(Point point);

}
