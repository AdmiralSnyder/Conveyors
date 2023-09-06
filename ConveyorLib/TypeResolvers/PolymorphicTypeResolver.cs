using ConveyorLib.Objects;
using CoreLib;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ConveyorLib.TypeResolvers;

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
