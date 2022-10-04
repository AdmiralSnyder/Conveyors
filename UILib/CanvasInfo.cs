using System.Windows.Controls;

namespace UILib;

public class CanvasInfo<TCanvas>
{
    public TCanvas Canvas { get; set; }
}

public class CanvasInfo : CanvasInfo<Canvas> { }
