using ConveyorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace ConveyorApp;

public static class CSharpOutputHelpers
{
    static CSharpOutputHelpers()
    {
        TypeOutputters = new();

        Register<double>(d => d.Out());
    }

    static Dictionary<int, Func<object, string>> ValueTupleResolver = new()
    {
        [2] = obj => OutVT2((dynamic)obj),
        [3] = obj => OutVT3((dynamic)obj),
        [4] = obj => OutVT4((dynamic)obj),
        [5] = obj => OutVT5((dynamic)obj),
        [6] = obj => OutVT6((dynamic)obj),
        [7] = obj => OutVT7((dynamic)obj),
        [8] = obj => OutVT8((dynamic)obj),
    };

    // these might be generated at some point...
    static string OutVT2<T1, T2>((T1 A, T2 B) t) => $"({t.A.Out()}, {t.B.Out()})";
    static string OutVT3<T1, T2, T3>((T1 A, T2 B, T3 C) t) => $"({t.A.Out()}, {t.B.Out()}, {t.C.Out()})";
    static string OutVT4<T1, T2, T3, T4>((T1 A, T2 B, T3 C, T4 D) t) => $"({t.A.Out()}, {t.B.Out()}, {t.C.Out()}, {t.D.Out()})";
    static string OutVT5<T1, T2, T3, T4, T5>((T1 A, T2 B, T3 C, T4 D, T5 E) t) => $"({t.A.Out()}, {t.B.Out()}, {t.C.Out()}, {t.D.Out()}, {t.E.Out()})";
    static string OutVT6<T1, T2, T3, T4, T5, T6>((T1 A, T2 B, T3 C, T4 D, T5 E, T6 F) t) => $"({t.A.Out()}, {t.B.Out()}, {t.C.Out()}, {t.D.Out()}, {t.E.Out()}, {t.F.Out()})";
    static string OutVT7<T1, T2, T3, T4, T5, T6, T7>((T1 A, T2 B, T3 C, T4 D, T5 E, T6 F, T7 G) t) => $"({t.A.Out()}, {t.B.Out()}, {t.C.Out()}, {t.D.Out()}, {t.E.Out()}, {t.F.Out()}, {t.G.Out()})";
    static string OutVT8<T1, T2, T3, T4, T5, T6, T7, T8>((T1 A, T2 B, T3 C, T4 D, T5 E, T6 F, T7 G, T8 H) t) => $"({t.A.Out()}, {t.B.Out()}, {t.C.Out()}, {t.D.Out()}, {t.E.Out()}, {t.F.Out()}, {t.G.Out()}, {t.H.Out()})";

    static bool TryGetValueTupleResolver(Type type, out Func<object, string> resolver) 
        => ValueTupleResolver.TryGetValue(type.GenericTypeArguments.Length, out resolver);

    public static void Register<T>(Func<T, string> func) => TypeOutputters[typeof(T)] = x => func((T)x);

    public static readonly Dictionary<Type, Func<object, string>> TypeOutputters;
        

    public static string Out<T>(this IEnumerable<T> items) => $"new {typeof(T).Name}[]{{{string.Join(", ", items.Select(i => i.Out()))}}}";

    public static string Out(this bool obj) => obj ? "true" : "false";

    public static string Out(this double value) => value.ToString(CultureInfo.InvariantCulture);

    public static string Out(this TwoPoints obj) => $"(({obj.P1.X}, {obj.P1.Y}), ({obj.P2.X}, {obj.P2.Y}))";
    public static string Out<T1, T2>(this (T1, T2) tuple) => $"({tuple.Item1.Out()}, {tuple.Item2.Out()})";

    public static string Out(this object? obj) => 
        (obj is not null 
        && ((obj.GetType() is { } type 
            && type.IsGenericType 
            && type.Namespace == "System" 
            && type.Name.StartsWith("ValueTuple`")
            && TryGetValueTupleResolver(type, out var outFunc))
        || TypeOutputters.TryGetValue(obj.GetType(), out outFunc)))
        ? outFunc(obj) 
        : (obj?.ToString()?? "null");

    public static string Out(this string str) => $@"""{str}""";
    public static string Out(this IAutomationOutByID obj) => $@"""{obj.ID}""";
}
