using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib;

public static class Func
{
    public static T Modify<T>(this T obj, Action<T> modifyAction)
    {
        modifyAction(obj);
        return obj;
    }

    private static T Modify<T, TArg>(this T obj, Action<T, TArg> modifyAction, TArg arg)
    {
        modifyAction(obj, arg);
        return obj;
    }

    public static void SetterInpc<T>(this INotifyPropertyChangedImpl obj, ref T backingField, T value, [CallerMemberName] string propertyName = "INVALID")
    {
        if (!object.Equals(backingField, value))
        {
            backingField = value;
            obj.OnPropertyChanged(propertyName);
        }
    }

    public static void Setter<T>(ref T backingField, T value, Action<T> onChangeAction)
    {
        if (!object.Equals(backingField, value))
        {
            backingField = value;
            onChangeAction(value);
        }
    }

    public static void SetterInpc<T>(this INotifyPropertyChangedImpl obj, ref T backingField, T value, Action<T> onChangeAction, [CallerMemberName] string propertyName = "INVALID")
    {
        if (!object.Equals(backingField, value))
        {
            backingField = value;
            onChangeAction(value);
            obj.OnPropertyChanged(propertyName);
        }
    }

    public static void Setter<T>(ref T backingField, T value, Action<T, T> onChangeAction)
    {
        var oldValue = backingField;
        if (!object.Equals(backingField, value))
        {
            backingField = value;
            onChangeAction(oldValue, value);
        }
    }

    public static void SetterInpc<T>(this INotifyPropertyChangedImpl obj, ref T backingField, T value, Action<T, T> onChangeAction, [CallerMemberName] string propertyName = "INVALID")
    {
        var oldValue = backingField;

        if (!object.Equals(backingField, value))
        {
            backingField = value;
            onChangeAction(oldValue, value);
            obj.OnPropertyChanged(propertyName);
        }
    }

    public static void Setter<T>(ref T backingField, T value, Action onChangeAction)
    {
        if (!object.Equals(backingField, value))
        {
            backingField = value;
            onChangeAction();
        }
    }

    public static void SetterInpc<T>(this INotifyPropertyChangedImpl obj, ref T backingField, T value, Action onChangeAction, [CallerMemberName] string propertyName = "INVALID")
    {
        if (!object.Equals(backingField, value))
        {
            backingField = value;
            onChangeAction();
            obj.OnPropertyChanged(propertyName);
        }
    }

    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> factory)
        where TKey : notnull
    {
        if (!dict.TryGetValue(key, out var value))
        {
            value = factory(key);
            dict[key] = value;
        }
        return value;
    }

    public static TValue AddInto<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, TValue value)
        where TKey : notnull
    {
        if (dict.TryGetValue(key, out var lst))
        {
            lst.Add(value);
        }
        else
        {
            dict[key] = [value];
        }
        return value;
    }

    public static void RemoveFrom<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, TValue value)
        where TKey : notnull
    {
        if (dict.TryGetValue(key, out var lst))
        {
            lst.Remove(value);
        }
    }

    public static Dictionary<char, Point[][]> TextLocations = new()
    {
        ['H'] = [[(0, 0), (0, 10)], [(0, 5), (5, 5)], [(5, 0), (5, 10)] ],
        ['A'] = [[(0, 10), (2.5, 0), (5, 10)], [(0, 5), (5, 5)]],
        ['P'] = [[(0, 10), (0, 0), (4, 0), (5, 2.5), (4, 5), (0, 5)]],
        ['Y'] = [[(0, 0), (2.5, 5), (5, 0)], [(2.5, 5), (2.5, 10)]],
        ['B'] = [[(0, 10), (0, 0), (4, 0), (5, 2.5), (4, 4.5), (0, 5), (4, 5.5), (5, 7.5), (4.5, 10), (0, 10)]],
        ['I'] = [[(2.5, 0), (2.5, 10)]],
        ['R'] = [[(0, 10), (0, 0), (4, 0), (5, 2.5), (4, 4.5), (0, 5), (5, 10)]],
        ['T'] = [[(2.5, 0), (2.5, 10)], [(0, 0), (5, 0)]],
        ['D'] = [[(0, 10), (0, 0), (4, 0), (5, 2.5), (5, 4.5), (5, 5.5), (5, 7.5), (4.5, 10), (0, 10)]],
        ['U'] = [[(0, 0), (0, 8), (2.5, 10), (5, 8), (5, 0)]],
        [' '] = [],
    };

    public static IEnumerable<Point[][]> GetTextLocations(string s) => s.Select(c => TextLocations[c]);

    public static IEnumerable<IEnumerable<Point[][]>> GetTextLocations(IEnumerable<string> strings) => strings.Select(GetTextLocations);

    public static IEnumerable<IEnumerable<Point[][]>> GetTextLocations(params string[] strings) => strings.Select(GetTextLocations);
}
