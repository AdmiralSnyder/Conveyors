using System;

namespace ConveyorApp;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class GeneratedAttribute : Attribute{ }
