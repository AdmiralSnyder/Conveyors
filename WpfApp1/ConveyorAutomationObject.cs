using ConveyorLib;
using ConveyorLib.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using WpfLib;
using System.Text.Json;
using System.IO;
using ConveyorLib.Wpf;
using AutomationLib;
using CoreLib;
using GenerationLib;

namespace ConveyorApp;

public interface IGeneratedConveyorAutomationObject: IAutomationRoot, IAutomationFeatures
{
    List<Conveyor> Conveyors { get; }
    
    IConveyorCanvasInfo CanvasInfo { get; }
    
    Conveyor AddConveyor(IEnumerable<Point> points, bool isRunning, int lanes);

    Circle AddCircleCenterRadius((Point Center, double Radius) centerRadius);
    Circle AddCircleDiameter((Point Point1, Point Point2) diameter);
    Circle AddCircleThreePoints((Point Point1, Point Point2, Point Point3) threePoints);
    //string Blub();

    Line AddLine(TwoPoints points);
    LineSegment AddLineSegment(TwoPoints points);

    PointObj AddPoint(Point point);

    /// <summary>
    /// Creates a <see cref="Fillet"/>
    /// The order of the points defines the direction of the arc.
    /// </summary>
    /// <param name="points"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    Fillet AddFillet(TwoPoints points, double radius);

    void MovePoint(ConveyorPoint conveyorPoint, Point point);

    void OffsetPoint(ConveyorPoint conveyorPoint, Point point);

    bool SaveCustom(string fileName);
    bool SaveJSON(string fileName);

    bool Load(string fileName);
}

[Generate2<IGeneratedConveyorAutomationObject>]
public partial class ConveyorAutomationObject : IAutomationRoot<ConveyorAppApplication>
{
    public List<IAppObject<ConveyorAppApplication>> AutomationObjects { get; } = new();

    [Generated]
    public void Init(object obj)
    {
        var tuple = ((Canvas Canvas, IConveyorShapeProvider ShapeProvider))obj;
        Conveyors = new();
        CanvasInfo = new ConveyorCanvasInfo() { Canvas = tuple.Canvas, ShapeProvider = tuple.ShapeProvider};
    }

    public partial Conveyor AddConveyor(IEnumerable<Point> points, bool isRunning, int lanes)
    {
        var conv = Conveyor.Create(points, isRunning, lanes);
        Conveyor.AddToCanvas(conv, CanvasInfo);
        Conveyors.Add(conv);
        AutomationObjects.Add(conv);
        return conv;
    }

    private T AddAppObject<T>(T appObject)
        where T : IAppObject<ConveyorAppApplication>
    {
        if (appObject is ICanAddToCanvas<IConveyorCanvasInfo> canvasable)
        {
            canvasable.AddToCanvas(CanvasInfo);
        }
        AutomationObjects.Add(appObject);
        return appObject;
    }

    public partial Circle AddCircleCenterRadius((Point Center, double Radius) centerRadius) => AddAppObject(Circle.Create(centerRadius));
    public partial Circle AddCircleDiameter((Point Point1, Point Point2) diameter) => AddAppObject(Circle.Create(diameter));
    public partial Circle AddCircleThreePoints((Point Point1, Point Point2, Point Point3) threePoints) => AddAppObject(Circle.Create(threePoints));


    public partial Line AddLine(TwoPoints points) => AddAppObject(Line.Create(points));
    public partial LineSegment AddLineSegment(TwoPoints points) => AddAppObject(LineSegment.Create(points));
    public partial PointObj AddPoint(Point point) => AddAppObject(PointObj.Create(point));

    public partial Fillet AddFillet(TwoPoints points, double radius) => AddAppObject(Fillet.Create((points, radius)));

    public partial void MovePoint(ConveyorPoint conveyorPoint, Point point) => conveyorPoint.Location = point;

    public partial void OffsetPoint(ConveyorPoint conveyorPoint, Point point) => conveyorPoint.Location += point;

    public partial bool SaveJSON(string fileName)
    {

        var json = JsonSerializer.Serialize(AutomationObjects, new JsonSerializerOptions()
        {
            TypeInfoResolver = new PolymorphicTypeResolver(),
        });
        File.WriteAllText(fileName, json);
        return true;
    }

    public partial bool SaveCustom(string fileName)
    {
        var storeObjects = AutomationObjects.OfType<IStorable>().Select(StorageManager.Store).ToList();
        var json = JsonSerializer.Serialize(storeObjects, new JsonSerializerOptions()
        {
            IncludeFields = true,
            TypeInfoResolver = new StorageObjectTypeResolver(),
        });
        File.WriteAllText(fileName, json);
        return true;
    }

    public partial bool Load(string filename)
    {
        var json = File.ReadAllText(filename);
        var items = JsonSerializer.Deserialize<List<JsonValueStorageObject>>(json, new JsonSerializerOptions { IncludeFields = true });
        foreach (var item in items)
        {
            AddAppObject(StorageManager.CreateAppObject(item));
        }
        return true;
    }
}