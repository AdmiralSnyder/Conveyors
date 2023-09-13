using System.Drawing;

namespace ConveyorLib.Shapes;

public static class ShapeSpecificationDefaults
{
    public static Color TemporaryColor { get; set; } = Color.Magenta;
    public static Color DebugColor { get; set; } = Color.Magenta;

    public static double DefaultLineStrokeThickness { get; set; } = 2;
    public static double DefaultDebugLineStrokeThickness { get; set; } = 1.5;
    public static double ThinStrokeThickness { get; set; } = 0.5;
    public static Color DefaultLineColor { get; set; } = Color.Beige;
    public static Color DefaultPointColor { get; set; } = Color.WhiteSmoke;
    public static Color DefaultCircleColor { get; set; } = Color.AntiqueWhite;
    public static double DefaultPointDiameter { get; set; } = 3;
}
