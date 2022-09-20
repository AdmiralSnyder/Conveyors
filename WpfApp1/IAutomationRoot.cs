﻿using ConveyorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfLib;

namespace ConveyorApp;

public interface IAutomationRoot
{
    public void Init(object obj);
}

public interface IAutomationFeatures { }

public interface IGeneratedConveyorAutomationObject: IAutomationRoot, IAutomationFeatures
{
    List<Conveyor> Conveyors { get; }
    ConveyorCanvasInfo CanvasInfo { get; }
    Conveyor AddConveyor(IEnumerable<Point> points, bool isRunning, int lanes);

    void MovePoint(ConveyorPoint conveyorPoint, Point point);

    void OffsetPoint(ConveyorPoint conveyorPoint, Point point);
}

public interface IAutomationContext
{
    bool IsAutomated { get; set; }
    public Action<string> LogAction { get; set; }
}

[Generate2<IGeneratedConveyorAutomationObject>]
public partial class ConveyorAutomationObject : IAutomationRoot
{
    [Generated]
    public void Init(object obj)
    {
        var tuple = ((Canvas Canvas, ConveyorShapeProvider ShapeProvider))obj;
        Conveyors = new();
        CanvasInfo = new() { Canvas = tuple.Canvas, ShapeProvider = tuple.ShapeProvider};
    }

    public partial Conveyor AddConveyor(IEnumerable<Point> points, bool isRunning, int lanes)
    {
        var conv = Conveyor.Create(points, isRunning, lanes);
        Conveyor.AddToCanvas(conv, CanvasInfo);
        Conveyors.Add(conv);
        return conv;
    }

    public partial void MovePoint(ConveyorPoint conveyorPoint, Point point)
    {
        conveyorPoint.Location = point;
    }

    public partial void OffsetPoint(ConveyorPoint conveyorPoint, Point point)
    {
        conveyorPoint.Location = conveyorPoint.Location + point;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class GeneratedAttribute : Attribute{ }

[AttributeUsage(AttributeTargets.Class)]
public class Generate2Attribute<T> : Attribute
where T : IAutomationFeatures
{ }

public static class CSharpOutputHelpers
{
    public static string Out<T>(this IEnumerable<T> items) => $"new {typeof(T).Name}[]{{{string.Join(", ", items.Select(i => i.Out()))}}}";

    public static string Out(this bool obj) => obj ? "true" : "false";

    public static string Out(this object? obj) => obj?.ToString() ?? "null";

    public static string Out(this IAutomationOutByID obj) => $@"""{obj.ID}""";
}
