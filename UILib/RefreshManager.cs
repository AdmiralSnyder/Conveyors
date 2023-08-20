using CoreLib;

namespace UILib;

public static class RefreshManager<TNotification>
    where TNotification : INotification<TNotification>
{
    private static Dictionary<TNotification, HashSet<IRefreshListener<TNotification>>> ListenerResolver = new();
    private static HashSet<IRefreshListener<TNotification>> RegisteredListeners = new();

    public static void DoARefresh(TNotification obj)
    {
        if (ListenerResolver.TryGetValue(obj, out var listeners))
        {
            // TODO maybe multithread?
            foreach (var listener in listeners)
            {
                listener.Notify(obj);
            }
        }
    }

    public static void RegisterRefreshListener(IRefreshListener<TNotification> listener) => RegisteredListeners.Add(listener);

    public static void UnRegisterRefreshListener(IRefreshListener<TNotification> listener)
    {
        if (RegisteredListeners.Remove(listener))
        {
            // maybe use an inverse dict here?
            foreach (var possibleListeners in ListenerResolver.Values)
            {
                possibleListeners.Remove(listener);
            }
        }
    }

    public static void UnRegisterObserverNotification(IRefreshListener<TNotification> listener, TNotification obj)
    {
        if (ListenerResolver.TryGetValue(obj, out var listeners))
        {
            listeners.Remove(listener);
            if (!listeners.Any())
            {
                ListenerResolver.Remove(obj);
            }
        }
    }

    public static void RegisterRefreshObserver(IRefreshListener<TNotification> listener, TNotification obj)
    {
        var listeners = ListenerResolver.GetOrAdd(obj, _ => new());
        listeners.Add(listener);
    }

    public static void RegisterObserver(IRefreshListener<TNotification> listener, object? obj) // <TType>
    {
        if (obj is TNotification refreshable)
        {
            RegisterRefreshObserver(listener, refreshable);
        }
    }

    public static void UnRegisterObserver(IRefreshListener<TNotification> listener, object? obj) // <TType>
    {
        if (obj is TNotification refreshable)
        {
            UnRegisterObserverNotification(listener, refreshable);
        }
    }
}

public interface IRefreshManagerWrapper<TNotification>
     where TNotification : INotification<TNotification>
{
    void RegisterRefreshObserver(IRefreshListener<TNotification> listener, TNotification obj) => RefreshManager<TNotification>.RegisterRefreshObserver(listener, obj);
    void DoARefresh(TNotification obj) => RefreshManager<TNotification>.DoARefresh(obj);
}

class RefreshManagerWrapper<TNotification> : IRefreshManagerWrapper<TNotification>
    where TNotification : INotification<TNotification>
{
    public static RefreshManagerWrapper<TNotification> Instance = new();
}