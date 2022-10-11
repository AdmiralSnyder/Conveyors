using ConveyorLib.Objects;
using PointDef.twopoints;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConveyorLib;

public class StorageObject
{
    public static StorageObject<T> Create<T>(Type targetType, T value) => new(targetType, value);
    public static DeserializedStorageObject<T> CreateDeserialized<TTargetType, T>(JsonElement element) => new(typeof(TTargetType), element.Deserialize<T>(new JsonSerializerOptions { IncludeFields = true }));
}

public abstract class TypedStorageObject : StorageObject
{
    public TypedStorageObject(Type type) => Type = type;
    public Type Type { get; set; }

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

public interface IStorable<TThis, TSource>
{
    static abstract TThis Create(TSource source);
}

public interface IStorable
{
    StorageObject Store();
}

public static class StorageManager
{
    private static HashSet<Type> _StorableTypes = new()
    {
        typeof(LineSegment),
        typeof(Line),
        typeof(Fillet),
    };

    public static StorageObject Store<TObject>(TObject obj) where TObject : IStorable => obj.Store();

    public static IEnumerable<Type> StorableTypes => _StorableTypes;

    private static readonly Dictionary<Type, Func<JsonValueStorageObject, TypedStorageObject>> DeserializerFuncs
        = StorableTypes.ToDictionary(type => type, CreateDeserializerFunc);
    //{
    //    [typeof(LineSegment)] = dso => StorageObject.Create<LineSegment, TwoPoints>(dso.Value),
    //    [typeof(Line)] = dso => StorageObject.Create<Line, TwoPoints>(dso.Value),
    //    [typeof(Fillet)] = dso => StorageObject.Create<Fillet, (TwoPoints, double)>(dso.Value),
    //};
    private static Dictionary<string, Func<JsonValueStorageObject, TypedStorageObject>> DeserializerFuncsByName =
        DeserializerFuncs.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value);

    private static Type GetIStorageSourceType(Type appObjectType)
    {
        var interfaceType = appObjectType.GetInterface($"{nameof(IStorable)}`2");
        var genericArguments = interfaceType.GetGenericArguments();

        if (genericArguments.Length != 2) throw new Exception($"invalid IStorable interface (arity != 2)");
        if (genericArguments[0] != appObjectType) throw new Exception($"invalid IStorable interface (type {genericArguments[0].FullName} != {appObjectType.FullName})");

        return genericArguments[1];
    }
    
    private static Func<JsonValueStorageObject, TypedStorageObject> CreateDeserializerFunc(Type type)
    {
        var sourceType = GetIStorageSourceType(type);
        var method = typeof(StorageObject).GetMethod(nameof(StorageObject.CreateDeserialized)).MakeGenericMethod(type, sourceType);
        return (JsonValueStorageObject jvso) => (TypedStorageObject)method.Invoke(null, new object[] { jvso.Value });
    }

    private static Func<TypedStorageObject, IAppObject<ConveyorAppApplication>> CreateSerializerFunc(Type type)
    {
        var sourceType = GetIStorageSourceType(type);
        var method = typeof(StorageManager).GetMethod(nameof(CreateAppObjectGeneric), BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(type, sourceType);
        return (TypedStorageObject tso) => (IAppObject<ConveyorAppApplication>)method.Invoke(null, new object[] { tso });
    }

    private static readonly Dictionary<Type, Func<TypedStorageObject, IAppObject<ConveyorAppApplication>>> ObjectCreators
        = StorableTypes.ToDictionary(type => type, CreateSerializerFunc);
    //{
    //    [typeof(LineSegment)] = CreateAppObject<LineSegment, TwoPoints>,
    //    [typeof(Line)] = CreateAppObject<Line, TwoPoints>,
    //    [typeof(Fillet)] = CreateAppObject<Fillet, (TwoPoints, double)>,
    //};

    public static IAppObject<ConveyorAppApplication> CreateAppObject(TypedStorageObject tso) => ObjectCreators[tso.Type](tso);

    public static IAppObject<ConveyorAppApplication> CreateAppObject(JsonValueStorageObject jvso) => CreateAppObject(Load(jvso));


    private static IAppObject<ConveyorAppApplication> CreateAppObjectGeneric<TObject, TSource>(TypedStorageObject tso)
        where TObject : IStorable<TObject, TSource>, IAppObject<ConveyorAppApplication>
        => CreateObject<TObject, TSource>(tso.GetValue<TSource>());

    private static IAppObject<ConveyorAppApplication> CreateObject<TObject, TSource>(TSource source)
        where TObject : IStorable<TObject, TSource>, IAppObject<ConveyorAppApplication>
        => TObject.Create(source);


    public static TypedStorageObject Load(JsonValueStorageObject dso) => DeserializerFuncsByName[dso.TargetType](dso);
}