namespace UILib;

public interface INotification { }

public interface INotification<TNotification> : INotification
    where TNotification : INotification<TNotification>
{
    IRefreshManagerWrapper<TNotification> Manager => RefreshManagerWrapper<TNotification>.Instance;
}