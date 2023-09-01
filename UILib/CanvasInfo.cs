using System.Windows.Controls;

namespace UILib;

public interface ICanvasInfo
{ 
    TShape AddToCanvas<TShape>(TShape shape);
}

public interface ICanvasInfo<TCanvas> : ICanvasInfo 
{
    public TCanvas Canvas { get; set; }
}

public abstract class CanvasInfo<TCanvas> : ICanvasInfo<TCanvas>
{
    public TCanvas Canvas { get; set; }

    public abstract TShape AddToCanvas<TShape>(TShape shape);
}