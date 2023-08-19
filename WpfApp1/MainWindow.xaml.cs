using ConveyorLib;
using CoreLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UILib;
using WpfLib;
using ConveyorApp.Inputters;
using CoreLib.Definition;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace ConveyorApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        ShapeProvider = new();
        ShapeProvider.RegisterSelectBehaviour(SelectShapeAction);
        this.DataContext = this;

        InitializeComponent();

        SelectionManager = new CanvasSelectionManager(TheCanvas);
        PickManager = new(TheCanvas);
        InputPickManager = new();

        CreationCommandManager = new();



        AutoRoot = ConveyorAutomationObject.CreateAutomationObject(out var context);
        AutoRoot.Init((TheCanvas, ShapeProvider));

        InputContext = new CanvasInputContext()
        {
            Canvas = TheCanvas,
            NotesLabel = NotesLabel,
            MainWindow = this,
        };

        CreationCommandManager.InputContext = InputContext;
        CreationCommandManager.AutoRoot = AutoRoot;

        CreationCommandManager.AddCommands(AddActionButton);

        context.LogAction = async s => await textEditor2.Dispatcher.InvokeAsync(() => textEditor2.AppendText(s + Environment.NewLine));
        ScriptRunner.InitializeScriptingEnvironment(AutoRoot, Dispatcher, RunB);

        DebugHelper.Instance = new ConveyorDebugHelper() { Canvas = TheCanvas, ShapeProvider = ShapeProvider };
    }

    public IGeneratedConveyorAutomationObject AutoRoot { get; }

    private void AddActionButton(Func<Task> commandAction, string commandName, string? caption)
    {
        var button = new Button() 
        { 
            Content = commandName == "Add Line Segment" 
                ? Resources["AddLineSegmentButtonContent"] 
                : caption ?? commandName, 
            ToolTip = commandName };
        button.Click += async (s, e) => await commandAction(); 
        ButtonsSP.Children.Add(button);
    }

    public CreationCommandManager CreationCommandManager { get; set; }
    public SelectionManager SelectionManager { get; set; }
    public CanvasPickManager PickManager { get; set; }

    public InputPickManager InputPickManager { get; set; }
    
    private void SelectShapeAction(Shape shape)
    {
        var mousePosition = Mouse.GetPosition(TheCanvas);

        var oldSelectedObject = SelectionManager.ChosenObject;

        if (shape.Tag is ISelectObject selectObject)
        {
            if (InputPickManager.IsActive)
            {
                InputPickManager.MousePosition = mousePosition;
                if (InputPickManager.QueryCanPickObject(selectObject))
                {
                    InputPickManager.ChosenObject = selectObject;
                }
            }
            else if (PickManager.IsActive)
            {
                PickManager.MousePosition = mousePosition;
                if (PickManager.QueryCanPickObject(selectObject))
                {
                    PickManager.ChosenObject = selectObject;
                }
            }
            else if (SelectionManager.IsActive)
            {
                SelectionManager.MousePosition = mousePosition;

                if (SelectionManager.HierarchicalSelection)
                {
                    SelectionManager.ChosenObject = selectObject.FindPredecessorInPath(oldSelectedObject);
                }
                else
                {
                    SelectionManager.ChosenObject = selectObject;
                }
            }
        }
    }

    public CanvasInputContext InputContext { get; private set; }

    #region Create Actions

    private async void AddCircleCenterRadiusB_Click(object sender, RoutedEventArgs e) => await CreationCommandManager.AddCircleCenterRadius();
    private async void AddCircle2PointsB_Click(object sender, RoutedEventArgs e) => await CreationCommandManager.AddCircleTwoPoints();
    private async void AddCircle3PointsB_Click(object sender, RoutedEventArgs e) => await CreationCommandManager.AddCircleThreePoints();
    private async void AddLineB_Click(object sender, RoutedEventArgs e) => await CreationCommandManager.AddLine();
    private async void AddLineSegmentB_Click(object sender, RoutedEventArgs e) => await CreationCommandManager.AddLineSegment();
    private async void AddPointB_Click(object sender, RoutedEventArgs e) => await CreationCommandManager.AddPoint();

    #endregion

    internal ConveyorShapeProvider ShapeProvider { get; set; }

    // TODO put the zoom functionality into a behaviour
    private void TheCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        var pos = e.GetPosition(TheCanvas);
        CanvasScaleTransform.CenterX = pos.X;
        CanvasScaleTransform.CenterY = pos.Y;
        if (e.Delta > 0)
        {
            CanvasScaleTransform.ScaleX *= 2;
            CanvasScaleTransform.ScaleY *= 2;
        }
        else
        {
            CanvasScaleTransform.ScaleX /= 2;
            CanvasScaleTransform.ScaleY /= 2;
        }
    }


    private void PutItemB_Click(object sender, RoutedEventArgs e)
    {
        foreach (var conveyor in AutoRoot.Conveyors)
        {
            conveyor.SpawnItems(ShapeProvider, FirstOnlyCB.IsChecked);
        }
    }

    private bool _IsRunning;
    public bool IsRunning
    {
        get => _IsRunning;
        set => Func.Setter(ref _IsRunning, value, isRunning => AutoRoot.Conveyors.ForEach(c => c.IsRunning = isRunning));
    }

    private void RunningCB_Click(object sender, RoutedEventArgs e) => IsRunning = RunningCB.IsChecked ?? false;

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => IsRunning = false;

    private async void MovePointB_Click(object sender, RoutedEventArgs e)
    {
        RunningCB.IsChecked = false;
        await MoveInputter.StartInput(InputContext);
    }

    private async void RunB_Click(object sender, RoutedEventArgs e) => await ScriptRunner.RunScript(textEditor.Text);


    private readonly ScriptRunner ScriptRunner = new();

    private void HappyBirthdayRubyB_Click(object sender, RoutedEventArgs e) => WriteString("R");
    //WriteString("""
    //HAPPY
    //BIRTHDAY
    //RUBY
    //""");

    private void WriteString(string text) => WriteStrings(text.Split(Environment.NewLine));

    private void WriteStrings(IEnumerable<string> lines)
    {
        int xOffset;
        int yOffset = 0;
        double scaling = 5;
        foreach (var wordCoords in Func.GetTextLocations(lines))
        {
            xOffset = 0;

            foreach (var charCoords in wordCoords)
            {
                foreach (var strokecoords in charCoords)
                {
                    AutoRoot.AddConveyor(strokecoords.Scale(scaling + yOffset).Add((xOffset * 60 * (1 + (yOffset * 0.2)) + 40, yOffset * 90 + 40)), true, yOffset + 2);
                }
                xOffset++;
            }

            yOffset++;
        }
    }

    private void DebugB_Click(object sender, RoutedEventArgs e)
    {
        //[InlineData(1, 1, 1, -1, 0, 1, 270)]

        LineDefinition line1 = new(((30, 30), (50, 60)));
        DebugHelper.PutLineSegmentVector(line1.RefPoints);
        LineDefinition line2 = new(line1.ReferencePoint1, line1.Vector.RotateAroundOrigin(30d.Degrees()));
        DebugHelper.PutLineSegmentVector(line2.RefPoints);
    }

    private void SaveB_Click(object sender, RoutedEventArgs e) => AutoRoot.SaveCustom(@"T:\conveyorApp\conveyorApp.json");

    private void LoadB_Click(object sender, RoutedEventArgs e) => AutoRoot.Load(@"T:\conveyorApp\conveyorApp.json");

    private void SelectB_Click(object sender, RoutedEventArgs e) => SelectionManager.ToggleSelectMode();

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && SelectionManager.IsActive)
        {
            SelectionManager.ToggleSelectMode();
        }
    }
}