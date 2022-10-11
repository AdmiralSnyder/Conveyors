using ConveyorLib;
using ConveyorLib.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfLib;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Text.Json.Serialization.Metadata;

namespace ConveyorApp;

public interface IAutomationRoot
{
    public void Init(object obj);
}



public interface IAutomationRoot<TApplication> : IAutomationRoot
    where TApplication : IApplication
{
    List<IAppObject<TApplication>> AutomationObjects { get; }
}


public interface IAutomationFeatures { }

public interface IGeneratedConveyorAutomationObject: IAutomationRoot, IAutomationFeatures
{
    List<Conveyor> Conveyors { get; }
    
    ConveyorCanvasInfo CanvasInfo { get; }
    
    Conveyor AddConveyor(IEnumerable<Point> points, bool isRunning, int lanes);
    
    Line AddLine(TwoPoints points);
    LineSegment AddLineSegment(TwoPoints points);

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

public interface IAutomationContext
{
    bool IsAutomated { get; set; }
    public Action<string> LogAction { get; set; }
}

[Generate2<IGeneratedConveyorAutomationObject>]
public partial class ConveyorAutomationObject : IAutomationRoot<ConveyorAppApplication>
{
    public List<IAppObject<ConveyorAppApplication>> AutomationObjects { get; } = new();

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
        AutomationObjects.Add(conv);
        return conv;
    }

    private T AddAppObject<T>(T appObject)
        where T : IAppObject<ConveyorAppApplication>
    {
        if (appObject is ICanAddToCanvas<ConveyorCanvasInfo> canvasable)
        {
            canvasable.AddToCanvas(CanvasInfo);
        }
        AutomationObjects.Add(appObject);
        return appObject;
    }

    public partial Line AddLine(TwoPoints points) => AddAppObject(Line.Create(points));
    public partial LineSegment AddLineSegment(TwoPoints points) => AddAppObject(LineSegment.Create(points));

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

public class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        Type basePointType = typeof(IAppObject);
        if (jsonTypeInfo.Type.IsInterface && jsonTypeInfo.Type.IsGenericType && basePointType.IsAssignableFrom(jsonTypeInfo.Type))
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$AppObjectType",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(LineSegment), nameof(LineSegment)),
                    new JsonDerivedType(typeof(Line), nameof(Line)),
                }
            };
        }

        return jsonTypeInfo;
    }
}

public class StorageObjectTypeResolver : DefaultJsonTypeInfoResolver
{

    private static List<JsonDerivedType> DerivedStorageObjects;
    public static List<JsonDerivedType> GetDerivedStorageObjects()
    {
        if (DerivedStorageObjects is null)
        {
            var types = StorageManager.StorableTypes;
            var differentTypes = new HashSet<Type>(types.Select(t => t.BaseType.GenericTypeArguments.Last()));
            var genericTypes = differentTypes.Select(t => typeof(StorageObject<>).MakeGenericType(t));
            DerivedStorageObjects = genericTypes.Select(t => new JsonDerivedType(t)).ToList();
        }
        return DerivedStorageObjects;
    }

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);
        if (jsonTypeInfo.Type == typeof(StorageObject))
        {
            var derivedTypes = GetDerivedStorageObjects();
            
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$AppObjectType",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
            };

            foreach (var derivedType in derivedTypes)
            {
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(derivedType);
            }
        }

        return jsonTypeInfo;
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

    public static string Out(this TwoPoints obj) => $"(({obj.P1.X}, {obj.P1.Y}), ({obj.P2.X}, {obj.P2.Y}))";

    public static string Out(this object? obj) => obj?.ToString() ?? "null";
    public static string Out(this string str) => $@"""{str}""";
    public static string Out(this IAutomationOutByID obj) => $@"""{obj.ID}""";
}
