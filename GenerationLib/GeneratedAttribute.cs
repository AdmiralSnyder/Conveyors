using System;

namespace GenerationLib;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class GeneratedAttribute : Attribute{ }
