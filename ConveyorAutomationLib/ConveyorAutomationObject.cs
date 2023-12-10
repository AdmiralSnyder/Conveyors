using ConveyorLib;
using ConveyorLib.Objects;
using ConveyorLib.Objects.Conveyor;
using System.Text.Json;
using AutomationLib;
using CoreLib;
using GenerationLib;
using ConveyorLib.TypeResolvers;
using UILib;
using System.Collections;

namespace ConveyorAutomationLib;

public interface IGeneratedConveyorAutomationObject: IAutomationRoot, IAutomationFeatures
{
    List<Conveyor> Conveyors { get; }
    //List<IAppObject> /*AutomationObjects*/ { get; }

    IEnumerable<ISelectObject> GetSelectObjects();
    
    IConveyorCanvasInfo CanvasInfo { get; }

    void SelectItem(string id);
    void ClearSelection();
    
    Conveyor AddConveyor(IEnumerable<Point> points, bool isRunning, int lanes);

    Circle AddCircleCenterRadius((Point Center, double Radius) centerRadius);
    Circle AddCircleDiameter((Point Point1, Point Point2) diameter);
    Circle AddCircleThreePoints((Point Point1, Point Point2, Point Point3) threePoints);

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

    FreeHandLine AddFreeHand(IEnumerable<Point> points);

    void MovePoint(ConveyorPoint conveyorPoint, Point point);
    void MovePoint((ConveyorPoint conveyorPoint, Point point) tuple);

    void OffsetPoint(ConveyorPoint conveyorPoint, Point point);
    void OffsetPoint((ConveyorPoint conveyorPoint, Point point) tuple);

    bool SaveCustom(string fileName);
    bool SaveJSON(string fileName);

    bool Load(string fileName);
}

[Generate2<IGeneratedConveyorAutomationObject>]
public partial class ConveyorAutomationObject : IAutomationRoot<ConveyorAppApplication>
{
    public List<IAppObject<ConveyorAppApplication>> AutomationObjects { get; } = [];

    public HashSet<IAppObject<ConveyorAppApplication>> SelectedObjects { get; } = [];

    [Generated]
    public void Init(object obj)
    {
        Conveyors = [];
        CanvasInfo = (IConveyorCanvasInfo)obj;

        IList conv = Conveyors;


        //...

        
        var convList = (List<Conveyor>)conv;

        var convList2 = conv as List<Conveyor>;
        if (convList2 is not null) { }

        if (conv is List<Conveyor> convList3) { }


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

    public partial FreeHandLine AddFreeHand(IEnumerable<Point> points) => AddAppObject(FreeHandLine.Create(points));

    public partial void MovePoint(ConveyorPoint conveyorPoint, Point point) => conveyorPoint.Location = point;
    public partial void MovePoint((ConveyorPoint conveyorPoint, Point point) info) => MovePoint(info.conveyorPoint, info.point);

    public partial void OffsetPoint(ConveyorPoint conveyorPoint, Point point) => conveyorPoint.Location += point;
    public partial void OffsetPoint((ConveyorPoint conveyorPoint, Point point) info) => OffsetPoint(info.conveyorPoint, info.point);

    public partial void SelectItem(string id)
    {
        ClearSelection();
        if (AutomationObjects.FirstOrDefault(x => x.ID == id) is { } obj)
        {
            SelectedObjects.Add(obj);
            CanvasInfo?.SelectionChanged();
        }
    }

    public partial void ClearSelection()
    {
        SelectedObjects.Clear();
        CanvasInfo?.SelectionChanged();
    }

    public partial IEnumerable<ISelectObject> GetSelectObjects() => SelectedObjects.OfType<ISelectObject>();

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