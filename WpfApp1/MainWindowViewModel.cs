using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using ConveyorAutomationLib;
using ConveyorLib.Shapes;
using ConveyorLib.Wpf;
using CoreLib;

namespace ConveyorApp;

public class MainWindowViewModel : INotifyPropertyChangedImpl
{
    public MainWindowViewModel()
    {
        ShapeProvider = new WpfConveyorShapeProvider();
        SelectionManager = new CanvasSelectionManager();
    }

    private string _StatusBarHelpText = "";

    public string StatusBarHelpText
    {
        get => _StatusBarHelpText;
        set => this.SetterInpc(ref _StatusBarHelpText, value);
    }

    private double _SnapGridWidth = 10;
    public double SnapGridWidth 
    {
        get => _SnapGridWidth; 
        set => this.SetterInpc(ref _SnapGridWidth, value); 
    }


    private Canvas _TheCanvas;

    public WpfCanvasInputContext InputContext { get; private set; }
    internal IConveyorShapeProvider ShapeProvider { get; set; }
    public SelectionManager SelectionManager { get; set; }
    public CanvasPickManager PickManager { get; set; }

    public InputPickManager InputPickManager { get; set; }

    public CreationCommandManager CreationCommandManager { get; set; }

    public Action<string> LogAction { get; set; }

    public Canvas TheCanvas
    {
        get => _TheCanvas;
        set => Func.Setter(ref _TheCanvas, value, theCanvas =>
        {
            InputContext = new WpfCanvasInputContext()
            {
                Canvas = new() { Canvas = theCanvas },

                ViewModel = this,
            };

            ((CanvasSelectionManager)SelectionManager).SetCanvas(TheCanvas);

            PickManager = new();
            PickManager.SetCanvas(TheCanvas);

            InputPickManager = new();

            CreationCommandManager = new();

            AutoRoot = ConveyorAutomationObject.CreateAutomationObject(out var context);
            
            AutoRoot.Init(new WpfConveyorCanvasInfo() { Canvas = TheCanvas, ShapeProvider = ShapeProvider});

            CreationCommandManager.InputContext = InputContext;
            CreationCommandManager.AutoRoot = AutoRoot;

            context.LogAction = s => LogAction?.Invoke(s);
        });
    }
    private bool _IsRunning;

    public bool IsRunning
    {
        get => _IsRunning;
        set => this.SetterInpc(ref _IsRunning, value, isRunning => AutoRoot.Conveyors.ForEach(c => c.IsRunning = isRunning));
    }

    public int LaneCount { get; set; } = 1;
    
    private Point _PanValue;
    public Point PanValue 
    {
        get => _PanValue;
        set => this.SetterInpc(ref _PanValue, value); 
    }

    internal Func<MouseEventArgs, Point> GetAbsolutePositionFunc { get; set; }
}
