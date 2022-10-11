using System.Windows.Controls;

namespace UILib;

public interface ICanvasInfo
{ }

public class CanvasInfo<TCanvas> : ICanvasInfo
{
    public TCanvas Canvas { get; set; }
}

public class CanvasInfo : CanvasInfo<Canvas> { }
