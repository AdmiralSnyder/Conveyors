using System;
using AutomationLib;

namespace GenerationLib;

[AttributeUsage(AttributeTargets.Class)]
public class Generate2Attribute<T> : Attribute
where T : IAutomationFeatures
{ }
