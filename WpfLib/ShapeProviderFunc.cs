using UILib.Shapes;

namespace WpfLib;

public static class ShapeProviderFunc
{
    public static T WithSelectBehaviour<T>(this T shape) where T : IShape
    {
        if (ShapeProvider.SelectBehaviour is { } sb)
        {
            shape.ApplyMouseBehavior(sb);
        }
        return shape;
    }
}
