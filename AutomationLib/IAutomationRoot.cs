using CoreLib;

namespace AutomationLib;

public interface IAutomationRoot
{
    public void Init(object obj);
}

public interface IAutomationRoot<TApplication> : IAutomationRoot
    where TApplication : IApplication
{
    List<IAppObject<TApplication>> AutomationObjects { get; }
}