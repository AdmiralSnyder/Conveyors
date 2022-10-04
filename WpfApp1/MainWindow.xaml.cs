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
        if ((await SelectLineInputter.Create(InputContext).StartAsync()).IsSuccess(out var line1))
        {
            if ((await SelectLineInputter.Create(InputContext).StartAsync()).IsSuccess(out var line2))
            {
                var angle = Maths.AngleBetween(line1.Vector, line2.Vector);

                if (Maths.GetCrossingPoint((line1.ReferencePoint1, line1.ReferencePoint2), (line2.ReferencePoint1, line2.ReferencePoint2), out var crossingPoint))
                {
                    AddPoint(crossingPoint);

                    var radius = 25d;

                    var tangent = Math.Tan(angle.CounterAngle().Radians / 2);

                    // tan alpha = a / b
                    // tan alpha * b = a
                    var a = tangent * radius;
                    var unitVector1 = line1.Vector.Normalize();
                    var start1 = crossingPoint + unitVector1.Multiply(a);

                    var unitVector2 = line2.Vector.Normalize();
                    var start2 = crossingPoint + unitVector2.Multiply(a);

                    //AddPoint(start1);
                    //AddPoint(start2);


                    bool largeArc = false;
                    SweepDirection swDir = SweepDirection.Clockwise;

                    var pg = new PathGeometry();

                    pg.Figures.Add(new()
                    {
                        StartPoint = start1,
                        Segments = { new ArcSegment(start2, new(radius, radius), 0, largeArc, swDir, true) }
                    });

                    var shape = ShapeProvider.CreateCircleSectorArc(pg, true);
                    TheCanvas.Children.Add(shape);

                    //AddCircle(crossingPoint, radius);

                }


                //Vector oStart = new(prevEnd, Point.Location);
                //var radius = oStart.Length();

                ////var radius = ConveyorSegment.LineDistance / 2;

                ////Vector oEnd = nextStart.Subtract(Point.Location);
                ////var oStartNorm = oStart.Normalize(oStartLen);
                ////var oEndNorm = oEnd.Normalize();
                ////var dotProd = oStartNorm.DotProduct(oEndNorm);

                //bool clockwise = !Point.IsClockwise;

                //// TODO correctly calculate the inside property.
                //Inside = IsLeft == clockwise;

                //var (largeArg, swDir) = (clockwise, IsLeft) switch
                //{
                //    // TODO inside
                //    (true, true) => (false, SweepDirection.Counterclockwise), // left turn, left side, bad
                //    (true, false) => (false, SweepDirection.Counterclockwise),  // right turn, right side, bad
                //                                                                // outside
                //    (false, true) => (false, SweepDirection.Clockwise), // right turn, left side, good
                //    (false, false) => (false, SweepDirection.Clockwise), // left turn, right side, good
                //};

                //if (Inside)
                //{
                //    var previousSegmentLane = (ConveyorSegmentLane)ElementsNode.Previous.Value;
                //    var P1 = previousSegmentLane.StartPoint;
                //    var P2 = previousSegmentLane.EndPoint;

                //    var nextSegmentLane = (ConveyorSegmentLane)ElementsNode.Next.Value;
                //    var R1 = nextSegmentLane.StartPoint;
                //    var R2 = nextSegmentLane.EndPoint;

                //    //if (previousSegmentLane.Length == 0 || nextSegmentLane.Length == 0 || Maths.VectorsAreParallel(new(P1, P2), new(R1, R2)) || Maths.VectorsAreInverseParallel(new(P1, P2), new(R1, R2)))
                //    //{
                //    //    if (ElementsNode.Previous?.Value is ConveyorSegmentLane prevSegLane)
                //    //    {
                //    //        prevSegLane.EndPoint = P2;
                //    //    }

                //    //    if (ElementsNode.Next?.Value is ConveyorSegmentLane nextSegLane)
                //    //    {
                //    //        nextSegLane.StartPoint = R1;
                //    //    }
                //    //}
                //    //else
                //    //{
                //    var yr1 = R1.Y;
                //    var xp = P2.X - P1.X;
                //    var yp1 = P1.Y;
                //    var xr1 = R1.X;
                //    var yp = P2.Y - P1.Y;
                //    var xp1 = P1.X;
                //    var xr = R2.X - R1.X;
                //    var yr = R2.Y - R1.Y;
                //    var quotient = (xr * yp - yr * xp);
                //    if (quotient == 0)
                //    {
                //        if (ElementsNode.Previous?.Value is ConveyorSegmentLane prevSegLane)
                //        {
                //            prevSegLane.EndPoint = P2;
                //        }

                //        if (ElementsNode.Next?.Value is ConveyorSegmentLane nextSegLane)
                //        {
                //            nextSegLane.StartPoint = R1;
                //        }
                //    }
                //    else
                //    {
                //        var sr = (yr1 * xp - yp1 * xp - xr1 * yp + xp1 * yp) / quotient; // TODO what happens if zero??

                //        var xq = xr1 + sr * (R2.X - R1.X);
                //        var yq = yr1 + sr * (R2.Y - R1.Y);

                //        var cross = new Vector(xq, yq);
                //        var start = R1;
                //        var end = P2;
                //        var CrossStart = start - cross;
                //        var CrossEnd = end - cross;
                //        bool x = true;
                //        Point ActStart = default;
                //        if (x)
                //        {
                //            ActStart = (start - CrossStart) - CrossEnd;
                //        }
                //        else
                //        {
                //            ActStart = CrossEnd;
                //        }
                //        var ActEnd = (end - CrossStart) - CrossEnd;

                //        ArcGeometry.Figures.Add(new()
                //        {
                //            StartPoint = ActStart,
                //            Segments = { new ArcSegment(ActEnd, new(radius, radius), Point.Angle.Degrees, largeArg, swDir, true) }
                //        });

                //        if (ElementsNode.Previous?.Value is ConveyorSegmentLane prevSegLane)
                //        {
                //            prevSegLane.EndPoint = ActStart;
                //        }

                //        if (ElementsNode.Next?.Value is ConveyorSegmentLane nextSegLane)
                //        {
                //            nextSegLane.StartPoint = ActEnd;
                //        }
                //    }
                //}
                //else
                //{
                //    ArcGeometry.Figures.Add(new()
                //    {
                //        StartPoint = prevEnd,
                //        Segments = { new ArcSegment(nextStart, new(radius, radius), Point.Angle.Degrees, largeArg, swDir, true) }
                //    });
                //}



                //Length = (Angle.HalfCircle - Point.AbsoluteAngle).Radians * radius;






                // if (!line1 || line2)
                //{
                //    CreateFillet();
                //}
            }
        }
    }
}
