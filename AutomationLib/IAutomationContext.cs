using System;

namespace AutomationLib;

public interface IAutomationContext
{
    bool IsAutomated { get; set; }
    public Action<string> LogAction { get; set; }
}
