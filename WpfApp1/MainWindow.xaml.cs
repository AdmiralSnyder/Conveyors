﻿using ConveyorLib;
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
using CoreLib.Definition;
using System.Windows.Controls;
using System.Threading.Tasks;
using UILib.Shapes;
using ScriptingLib;
using Microsoft.AspNetCore.SignalR.Client;
using WpfLib.Behaviors;
using ConveyorInputLib.Inputters;

namespace ConveyorApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        ViewModel = new() /*{ MainWindow = this}*/;
        this.DataContext = ViewModel;

        SelectBehaviorProviderInstanceWpf.RegisterSelectBehavior(SelectShapeAction);

        InitializeComponent();

        ViewModel.TheCanvas = TheCanvas;
        ViewModel.GetAbsolutePositionFunc = e => e.GetPosition(this).AsPoint();

        ViewModel.CreationCommandManager.AddCommands(AddActionButton);

        ScriptRunner.InitializeScriptingEnvironment(AutoRoot, 
            async () => await RunB.Dispatcher.InvokeAsync(() => RunB.IsEnabled = true),
            async () => await Dispatcher.InvokeAsync(() => Mouse.OverrideCursor = Cursors.Wait),
            async () => await Dispatcher.InvokeAsync(() => Mouse.OverrideCursor = null),
            ex => MessageBox.Show(ex.ToString(), "Error running script"),
        new[] { typeof(Point), typeof(ConveyorAutomationLib.ConveyorAutomationObject) }, 
        new[] { typeof(Point), typeof(ConveyorAutomationLib.ConveyorAutomationObject) });

        ViewModel.LogAction = async s => await textEditor2.Dispatcher.InvokeAsync(() => textEditor2.AppendText(s + Environment.NewLine));


        DebugHelper.Instance = new ConveyorDebugHelper() { Canvas = TheCanvas, ShapeProvider = ViewModel.ShapeProvider };
    }

    public MainWindowViewModel ViewModel { get; set; }

    private void AddActionButton(Func<Task> commandAction, string commandName, string? caption)
    {
        var button = new Button()
        {
            Content = commandName == "Add Line Segment"
                ? Resources["AddLineSegmentButtonContent"]
                : caption ?? commandName,
            ToolTip = commandName
        };
        button.Click += async (s, e) => await commandAction();
        ButtonsSP.Children.Add(button);
    }

    private void SelectShapeAction(IShape shape)
    {
        var mousePosition = Mouse.GetPosition(TheCanvas);

        if (shape.Tag is ISelectObject selectObject)
        {
            if (ViewModel.InputPickManager is { IsActive: true } inpMgr)
            {
                inpMgr.MousePosition = mousePosition;
                if (inpMgr.QueryCanPickObject(selectObject))
                {
                    inpMgr.ChosenObject = selectObject;
                }
            }
            else if (ViewModel.PickManager is { IsActive: true } pickMgr)
            {
                pickMgr.MousePosition = mousePosition;
                if (pickMgr.QueryCanPickObject(selectObject))
                {
                    pickMgr.ChosenObject = selectObject;
                }
            }
            else if (ViewModel.SelectionManager is { IsActive: true } selMgr)
            {
                selMgr.MousePosition = mousePosition;

                if (selMgr.HierarchicalSelection)
                {
                    selMgr.ChosenObject = selectObject.FindPredecessorInPath(selMgr.ChosenObject);
                }
                else
                {
                    selMgr.ChosenObject = selectObject;
                }
            }
        }
    }

    // TODO put the zoom functionality into a behavior
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

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => ViewModel.IsRunning = false;

    private async void MovePointB_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.IsRunning = false;
        await MoveConveyorPointInputter.StartInputOnce(ViewModel.InputContext).Then(AutoRoot.MovePoint);
    }

    private async void RunB_Click(object sender, RoutedEventArgs e) => await ScriptRunner.RunScript(textEditor.Text);


    private readonly ScriptRunner ScriptRunner = new();

    private void HappyBirthdayRubyB_Click(object sender, RoutedEventArgs e) => WriteString("""
    HAPPY
    BIRTHDAY
    RUBY
    """);

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

    private void SelectB_Click(object sender, RoutedEventArgs e) => ViewModel.SelectionManager.ToggleSelectMode();

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && ViewModel.SelectionManager.IsActive)
        {
            ViewModel.SelectionManager.ToggleSelectMode();
        }
    }

    private async void SendB_Click(object sender, RoutedEventArgs e)
    {
        var conn = new HubConnectionBuilder().WithUrl("http://localhost:5262/conveyorhub").Build();
        await conn.StartAsync();
        await conn.SendAsync("SendMessage", "Hello", textEditor.Text);
    }
}