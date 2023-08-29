using System;
using System.ComponentModel;
using System.Windows.Controls;
using ConveyorLib;
using CoreLib;

namespace ConveyorApp;

public class MainWindowViewModel : INotifyPropertyChangedImpl
{
    public MainWindowViewModel()
    {
        ShapeProvider = new();
        SelectionManager = new CanvasSelectionManager();

    }

    private string _StatusBarHelpText = "";

    public string StatusBarHelpText
    {
        get => _StatusBarHelpText;
        set => this.SetterInpc(ref _StatusBarHelpText, value);
    }

    private Canvas _TheCanvas;

    public CanvasInputContext InputContext { get; private set; }
    internal ConveyorShapeProvider ShapeProvider { get; set; }
    public SelectionManager SelectionManager { get; set; }
    public CanvasPickManager PickManager { get; set; }

    public InputPickManager InputPickManager { get; set; }

    public CreationCommandManager CreationCommandManager { get; set; }

    public IGeneratedConveyorAutomationObject AutoRoot { get; private set; }

    public Action<string> LogAction { get; set; }

    // TODO needs to be deleted
    public MainWindow MainWindow { get; set; }

    public Canvas TheCanvas
    {
        get => _TheCanvas;
        set => Func.Setter(ref _TheCanvas, value, theCanvas =>
        {
            InputContext = new CanvasInputContext()
            {
                Canvas = theCanvas,
                ViewModel = this,
                MainWindow = MainWindow,
            };

            ((CanvasSelectionManager)SelectionManager).SetCanvas(TheCanvas);

            PickManager = new();
            PickManager.SetCanvas(TheCanvas);

            InputPickManager = new();

            CreationCommandManager = new();

            AutoRoot = ConveyorAutomationObject.CreateAutomationObject(out var context);
            
            AutoRoot.Init((TheCanvas, ShapeProvider));

            CreationCommandManager.InputContext = InputContext;
            CreationCommandManager.AutoRoot = AutoRoot;

            context.LogAction = s => LogAction?.Invoke(s);


        });
    }
}
