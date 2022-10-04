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

        SelectionManager = new()
        {
            UpdateBoundingBox = ShowSelectionBoundingBox
        };

        PickManager = new()
        {
            UpdateBoundingBox = ShowPickBoundingBox
        };

        InitializeComponent();


        AutoRoot = ConveyorAutomationObject.CreateAutomationObject(out var context);
        AutoRoot.Init((TheCanvas, ShapeProvider));

        InputContext = new CanvasInputContext()
        {
            Canvas = TheCanvas,
            NotesLabel = NotesLabel,
            MainWindow = this,
        };

        context.LogAction = s => textEditor2.Dispatcher.Invoke(() => textEditor2.AppendText(s + Environment.NewLine));
        ScriptRunner.InitializeScriptingEnvironment(AutoRoot, Dispatcher, RunB);

    }

    public IGeneratedConveyorAutomationObject AutoRoot { get; }

    public PickManager PickManager { get; set; }

    public SelectionManager SelectionManager { get; set; }

    private void SelectShapeAction(Shape shape)
    {
        var oldSelectedObject = SelectionManager.ChosenObject;

        if (shape.Tag is ISelectObject selectObject)
        {
            if (PickManager.IsActive)
            {
                if (PickManager.QueryCanPickObject(selectObject))
                {
                    PickManager.ChosenObject = selectObject;
                }
            }
            else if (SelectionManager.IsActive)
            {
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
    private Rectangle PickRect;

    private void ShowPickBoundingBox(ISelectObject? selectObject)
    {
        if (PickRect is not null)
        {
            TheCanvas.Children.Remove(PickRect);
        }
        if (selectObject is null) return;

        var boundingRect = Maths.GetBoundingRectTopLeftSize(selectObject.SelectionBoundsPoints);
        PickRect = new()
        {
            Width = boundingRect.P2.X + 8,
            Height = boundingRect.P2.Y + 8,
            Stroke = Brushes.Chartreuse,
            StrokeDashArray = new(new[] { 4d, 4d }),
            SnapsToDevicePixels = true,
            RadiusX = 2,
            RadiusY = 2,
        };
        PickRect.SetLocation(boundingRect.P1.Subtract((4, 4)));
        TheCanvas.Children.Add(PickRect);
    }

    private Rectangle SelectionRect;

    private void ShowSelectionBoundingBox(ISelectObject? selectObject)
    {
        if (SelectionRect is not null)
        {
            TheCanvas.Children.Remove(SelectionRect);
        }
        if (selectObject is null) return;

        var boundingRect = Maths.GetBoundingRectTopLeftSize(selectObject.SelectionBoundsPoints);
        SelectionRect = new()
        {
            Width = boundingRect.P2.X + 8,
            Height = boundingRect.P2.Y + 8,
            Stroke = Brushes.Moccasin,
            StrokeDashArray = new(new[] { 1d, 2d }),
            SnapsToDevicePixels = true,
            RadiusX = 2,
            RadiusY = 2,
        };
        SelectionRect.SetLocation(boundingRect.P1.Subtract((4, 4)));
        TheCanvas.Children.Add(SelectionRect);
    }

    internal Inputter CurrentInputter { get; set; }

    public CanvasInputContext InputContext { get; private set; }

    private async void AddPointB_Click(object sender, RoutedEventArgs e)
    {
        if ((await PointInputter.StartInput(InputContext, ShowMouseLocationInputHelper.Create(InputContext))).IsSuccess(out var point))
        {
            AddPoint(point);
        }
    }

    private Shape AddPoint(Point point)
    {
        var pointShape = ShapeProvider.CreatePoint(point);
        TheCanvas.Children.Add(pointShape);
        return pointShape;
    }

    private Shape AddCircle(Point center, double radius)
    {
        var circleShape = ShapeProvider.CreateCircle(center, radius);
        TheCanvas.Children.Add(circleShape);
        return circleShape;
    }

    private void AddConveyorB_Click(object sender, RoutedEventArgs e)
    {
        (CurrentInputter = ConveyorInputter.Create(InputContext)).Start();
    }

    private async void AddCircleCenterRadiusB_Click(object sender, RoutedEventArgs e)
    {
        if ((await CircleCenterRadiusInputter.StartInput(InputContext)).IsSuccess(out var info))
        {
            AddCircle(info.Center, info.Radius);
        }
    }

    private async void AddCircle3PointsB_Click(object sender, RoutedEventArgs e)
    {
        if ((await CircleThreePointsInputter.StartInput(InputContext)).IsSuccess(out var info)
            && Maths.GetCircleInfo(info, out var circInfo))
        {
            AddCircle(circInfo.Center, circInfo.Radius);
        }
    }

    private void TheCanvas_MouseDown(object sender, MouseButtonEventArgs e) => InputContext.HandleMouseDown(sender, e);

    internal ConveyorShapeProvider ShapeProvider { get; set; }

    private void TheCanvas_MouseMove(object sender, MouseEventArgs e) => InputContext.HandleMouseMove(sender, e);

    private void TheCanvas_MouseUp(object sender, MouseButtonEventArgs e) => InputContext.HandleMouseUp(sender, e);

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

    private void MovePointB_Click(object sender, RoutedEventArgs e)
    {
        RunningCB.IsChecked = false;
        (CurrentInputter = MoveInputter.Create(this.InputContext)).Start();
    }



    private void SelectB_Click(object sender, RoutedEventArgs e) => SelectionManager.ToggleSelectMode();


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

    private async void AddCircle2PointsB_Click(object sender, RoutedEventArgs e)
    {
        if ((await CircleDiameterInputter.StartInput(InputContext)).IsSuccess(out var info)
            && Maths.GetCircleInfoByDiameter(info, out var circInfo))
        {
            AddCircle(circInfo.Center, circInfo.Radius);
        }
    }

    private async void AddLineB_Click(object sender, RoutedEventArgs e)
    {
        if ((await LineInputter.Create(InputContext).StartAsync()).IsSuccess(out var points))
        {
            AutoRoot.AddLine(points);
        }
    }

    private async void AddFilletB_Click(object sender, RoutedEventArgs e)
    {
        if ((await FilletInfoInputter.Create(InputContext).StartAsync()).IsSuccess(out var lines))
        {
            if (Maths.CreateFilletInfo(lines.Item1.RefPoints, lines.Item2.RefPoints, out var filletInfo))
            {
                AutoRoot.AddFillet(filletInfo.Points, filletInfo.Radius);
            }
        }
    }
}
