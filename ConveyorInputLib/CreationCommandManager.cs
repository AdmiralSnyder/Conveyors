using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ConveyorApp.Inputters;
using ConveyorAutomationLib;
using ConveyorInputLib.Helpers;
using ConveyorInputLib.Inputters;
using InputLib;

namespace ConveyorInputLib;

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
    };

    protected Dictionary<string, (Func<Task> Command, string? Caption)> Commands { get; }

    public void AddCommands(Action<Func<Task>, string, string?> addActionButton)
    {
        foreach (var command in Commands)
        {
            addActionButton(command.Value.Command, command.Key, command.Value.Caption);
        }
    }

    public InputContextBase InputContext { get; set; }

    public IGeneratedConveyorAutomationObject AutoRoot { get; set; }

    public async Task AddCircleCenterRadius()
    {
        if ((await CircleCenterRadiusInputter.StartInput(InputContext)).IsSuccess(out var info))
        {
            AutoRoot.AddCircleCenterRadius(info);
        }
    }

    public async Task AddCircleThreePoints()
    {
        if ((await CircleThreePointsInputter.StartInput(InputContext)).IsSuccess(out var info))
        {
            AutoRoot.AddCircleThreePoints(info);
        }
    }

    public async Task AddCircleTwoPoints()
    {
        if ((await CircleDiameterInputter.StartInput(InputContext)).IsSuccess(out var info))
        {
            AutoRoot.AddCircleDiameter(info);
        }
    }

    public async Task AddLine()
    {
        if ((await LineInputter.Create(InputContext).StartAsync()).IsSuccess(out var points))
        {
            AutoRoot.AddLine(points);
        }
    }

    public async Task AddLineSegment()
    {
        if ((await LineInputter.Create(InputContext).StartAsync()).IsSuccess(out var points))
        {
            AutoRoot.AddLineSegment(points);
        }
    }

    public async Task AddPoint()
    {
        if ((await PointInputter.StartInput(InputContext, ShowMouseLocationInputHelper.Create(InputContext))).IsSuccess(out var point))
        {
            AutoRoot.AddPoint(point);
        }
    }
}
