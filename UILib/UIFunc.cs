using System.Diagnostics.CodeAnalysis;
using CoreLib.Definition;
using PointDef;
using UILib.Shapes;

namespace UILib;

public static class UIFunc
{
    public static Vector GetSize<TShape>(this TShape shape) where TShape : IShape => UIHelpers.GetSize(shape);
    public static TShape SetCenterLocation<TShape>(this TShape shape, Point location) where TShape : IShape
        => UIHelpers.SetLocation(shape, (location.Subtract(UIHelpers.GetSize(shape).Divide(2))));

    public static bool GetCircleDefinition(this IEllipse shape, [NotNullWhen(true)] out CircleDefinition? circleDefinition)
    {
        if (TryGetLocation(shape, out var location))
        {
            var size = GetSize(shape);

            if (size.X > 0)
            {
                circleDefinition = new(location + (size.X / 2, 0), location + (size.X / 2, size.X));

                return true;
            }
        }
        circleDefinition = default;

        return false;
    }


    public static TShape SetLocation<TShape>(this TShape shape, Point location) where TShape : IShape
        => UIHelpers.SetLocation(shape, location);

    public static TShape SetLocation<TShape>(this TShape shape, TwoPoints location) where TShape : IShape
        => UIHelpers.SetLocation(shape, location);

    public static bool TryGetLocation<TShape>(this TShape shape, out Point location) where TShape : IShape
        => UIHelpers.TryGetLocation(shape, out location);
}
