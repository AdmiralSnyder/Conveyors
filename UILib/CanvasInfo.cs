﻿using UILib.Shapes;

namespace UILib;

public interface ICanvasInfo
{ 
    TShape AddToCanvas<TShape>(TShape shape);
    void BeginInvoke<T>(IShape shape, Action<T> action, T value);
}

public interface ICanvasInfo<TCanvas> : ICanvasInfo 
{
    public TCanvas Canvas { get; set; }
}

public abstract class CanvasInfo<TCanvas> : ICanvasInfo<TCanvas>
{
    public TCanvas Canvas { get; set; }

    public abstract TShape AddToCanvas<TShape>(TShape shape);
    public abstract void BeginInvoke<T>(IShape shape, Action<T> action, T value);
    public abstract TShape RemoveFromCanvas<TShape>(TShape shape);
}