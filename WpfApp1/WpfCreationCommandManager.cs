using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConveyorApp.Inputters;
using ConveyorInputLib;

namespace ConveyorApp;

public class WpfCreationCommandManager : CreationCommandManager
{
    public new static List<string> CommandLabels 
        => StaticCommands.Select(kvp => kvp.Key.Item2 ?? kvp.Key.Item1)
        .Concat(StaticCommandsWpf.Select(kvp => kvp.Key.Item2 ?? kvp.Key.Item1))
        .ToList();

    public WpfCreationCommandManager()
    {
        foreach(var x in StaticCommandsWpf)
        {
            Commands.Add(x.Key.Item1, (x.Value(this), x.Key.Item2));
        }
    }

    public static Dictionary<(string, string?), Func<WpfCreationCommandManager, Func<Task>>> StaticCommandsWpf = new()
    {
        [("Add Fillet", "U")] = c => c.AddFillet,
        [("Add Conveyor", @"\___/")] = c => c.AddConveyor,
    };

    internal async Task AddConveyor()
    {
        if ((await ConveyorInputter.StartInput(InputContext)).IsSuccess(out var points))
        {
            AutoRoot.AddConveyor(points, ((WpfCanvasInputContext)InputContext).ViewModel.IsRunning, 
                ((WpfCanvasInputContext)InputContext).ViewModel.LaneCount);
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
}
