using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ConveyorApp.Inputters;
using ConveyorApp.Inputters.Helpers;
using ConveyorAutomationLib;

namespace ConveyorApp;

public class CreationCommandManager
{
    public CreationCommandManager()
    {
        Commands = StaticCommands.ToDictionary(kvp => kvp.Key.Item1, kvp => (kvp.Value(this), kvp.Key.Item2));
    }

    public static List<string> CommandLabels => StaticCommands.Select(kvp => kvp.Key.Item2 ?? kvp.Key.Item1).ToList();

    public static Dictionary<(string, string?), Func<CreationCommandManager, Func<Task>>> StaticCommands = new()
    {
        [("Add Point", ".")] = c => c.AddPoint,
        [("Add Line", "―")] = c => c.AddLine,
        [("Add Line Segment", null)] = c => c.AddLineSegment,
        [("Add Circle by Center+Circ Point", "O1")] = c => c.AddCircleCenterRadius,
        [("Add Circle by Diameter Points", "O2")] = c => c.AddCircleTwoPoints,
        [("Add Circle by Three Points", "O3")] = c => c.AddCircleThreePoints,
        [("Add Fillet", "U")] = c => c.AddFillet,
        [("Add Conveyor", @"\___/")] = c => c.AddConveyor,
    };

    private Dictionary<string, (Func<Task> Command, string? Caption)> Commands;
    internal void AddCommands(Action<Func<Task>, string, string?> addActionButton)
    {
        foreach (var command in Commands)
        {
            addActionButton(command.Value.Command, command.Key, command.Value.Caption);
        }
    }

    public CanvasInputContext InputContext { get; internal set; }
    public IGeneratedConveyorAutomationObject AutoRoot { get; internal set; }

    internal async Task AddCircleCenterRadius()
    {
        if ((await CircleCenterRadiusInputter.StartInput(InputContext)).IsSuccess(out var info))
        {
            AutoRoot.AddCircleCenterRadius(info);
        }
    }

    internal async Task AddCircleThreePoints()
    {
        if ((await CircleThreePointsInputter.StartInput(InputContext)).IsSuccess(out var info))
        {
            AutoRoot.AddCircleThreePoints(info);
        }
    }

    internal async Task AddCircleTwoPoints()
    {
        if ((await CircleDiameterInputter.StartInput(InputContext)).IsSuccess(out var info))
        {
            AutoRoot.AddCircleDiameter(info);
        }
    }



    internal async Task AddLine()
    {
        if ((await LineInputter.Create(InputContext).StartAsync()).IsSuccess(out var points))
        {
            AutoRoot.AddLine(points);
        }
    }

    internal async Task AddLineSegment()
    {
        if ((await LineInputter.Create(InputContext).StartAsync()).IsSuccess(out var points))
        {
            AutoRoot.AddLineSegment(points);
        }
    }

    internal async Task AddPoint()
    {
        if ((await PointInputter.StartInput(InputContext, ShowMouseLocationInputHelper.Create(InputContext))).IsSuccess(out var point))
        {
            AutoRoot.AddPoint(point);
        }
    }

    internal async Task AddFillet()
    {
        if ((await FilletInfoInputter.StartInput(InputContext)).IsSuccess(out var linesWithPoints))
        {
            var point1 = linesWithPoints.LineInfo1.Point;
            var point2 = linesWithPoints.LineInfo2.Point;
            if (Maths.CreateFilletInfo(linesWithPoints.LineInfo1.LineDefinition, linesWithPoints.LineInfo2.LineDefinition, (point1, point2), out var filletInfo))
            {
                AutoRoot.AddFillet(filletInfo.Points, filletInfo.Radius);
            }
        }
    }

    internal async Task AddConveyor()
    {
        if ((await ConveyorInputter.StartInput(InputContext)).IsSuccess(out var points))
        {
            AutoRoot.AddConveyor(points, InputContext.ViewModel.IsRunning, InputContext.ViewModel.LaneCount);
        }
    }
}
