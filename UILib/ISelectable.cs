namespace UILib;

public interface ISelectable : INotification<ISelectable>
{
    void NotifyRefresh() => Manager.DoARefresh(this);
}
