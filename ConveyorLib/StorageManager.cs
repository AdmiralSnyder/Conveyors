using ConveyorLib.Objects;
using PointDef.twopoints;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConveyorLib;

public class StorageObject
{
    public static StorageObject<T> Create<T>(Type targetType, T value) => new(targetType, value);
    public static DeserializedStorageObject<T> Create<TTargetType, T>(JsonElement element) => new(typeof(TTargetType), element.Deserialize<T>(new JsonSerializerOptions { IncludeFields = true }));
}

public abstract class TypedStorageObject : StorageObject
{
    public TypedStorageObject(Type type) => Type = type;
    public Type Type { get; set; }

    public Func<IAppObject<ConveyorAppApplication>> Create<T>(Func<T, IAppObject<ConveyorAppApplication>> createFunc) => () => createFunc(GetValue<T>());

    public abstract T GetValue<T>();
}

public class StorageObject2 : StorageObject
{
    public string TargetType { get; set; }
}

public class DeserializedStorageObject<TValue> : TypedStorageObject
{
    public DeserializedStorageObject(Type type, TValue value) : base(type)
    {
        Value = value;
    }

    public TValue Value { get; set; }

    public override T GetValue<T>() => (T)(object)Value;
}

public class JsonValueStorageObject : StorageObject2
{
    public JsonElement Value { get; set; }
}

public class StorageObject<T> : StorageObject2
{
    public StorageObject(Type targetType, T value)
    {
        Value = value;
        TargetType = targetType.Name;
    }

    public T Value { get; set; }
}

public interface IStorable
{
    StorageObject Store();
}

public static class StorageManager 
{
    public static StorageObject Store<TObject>(TObject obj)
        where TObject : IStorable
    {
        return obj.Store();
    }

    private static Dictionary<Type, Func<JsonValueStorageObject, TypedStorageObject>> DeserializerFuncs = new()
    {
        [typeof(LineSegment)] = dso => StorageObject.Create<LineSegment, TwoPoints>(dso.Value),
        [typeof(Line)] = dso => StorageObject.Create<Line, TwoPoints>(dso.Value),
        [typeof(Fillet)] = dso => StorageObject.Create<Fillet, (TwoPoints, double)>(dso.Value),
    };

    public static Dictionary<Type, Func<TypedStorageObject, IAppObject<ConveyorAppApplication>>> ObjectCreators = new()
    {
        [typeof(LineSegment)] = tso => tso.Create<TwoPoints>(LineSegment.Create)(),
        [typeof(Line)] = tso => tso.Create<TwoPoints>(Line.Create)(),
        [typeof(Fillet)] = tso => tso.Create<(TwoPoints, double)>(Fillet.Create)(),
    };

    private static Dictionary<string, Func<JsonValueStorageObject, TypedStorageObject>> DeserializerFuncs2 = DeserializerFuncs.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value);

    public static TypedStorageObject Load(JsonValueStorageObject dso) => DeserializerFuncs2[dso.TargetType](dso);
}