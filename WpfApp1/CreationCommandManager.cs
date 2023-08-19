using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ConveyorApp.Inputters;

namespace ConveyorApp;

public class CreationCommandManager
{
    public CreationCommandManager()
    {
        Commands = new()
        {
            ["Add Point"] = (AddPoint, "."),
            ["Add Line"] = (AddLine, "―"),
            ["Add Line Segment"] = (AddLineSegment, null),
            ["Add Circle by Center+Circ Point"] = (AddCircleCenterRadius, "O1"),
            ["Add Circle by Diameter Points"] = (AddCircleTwoPoints, "O2"),
            ["Add Circle by Three Points"] = (AddCircleThreePoints, "O3"),
            ["Add Fillet"] = (AddFillet, "U"),
            ["Add Conveyor"] = (AddConveyor, @"\__/"),
        };
    }

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
            AutoRoot.AddConveyor(points, InputContext.MainWindow.IsRunning, int.TryParse(InputContext.MainWindow.LanesCountTB.Text, out var lanesCnt) ? Math.Max(lanesCnt, 1) : 1);
        }
    }
}
