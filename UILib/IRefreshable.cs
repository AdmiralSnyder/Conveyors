using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UILib;

public interface IRefreshable : INotification<IRefreshable>
{
    void NotifyRefresh() => Manager.DoARefresh(this);
}

public interface IRefreshListener<TNotification>
    where TNotification : INotification<TNotification>
{
    void Register() => RefreshManager<TNotification>.RegisterRefreshListener(this);
    void Register(TNotification obj) => RefreshManager<TNotification>.RegisterRefreshObserver(this, obj);
    void Notify(TNotification obj);
}
