using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ConveyorLib.TypeResolvers;

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
