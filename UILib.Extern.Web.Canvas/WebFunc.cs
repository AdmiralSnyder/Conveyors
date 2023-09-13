using System.Drawing;

namespace UILib.Extern.Web.Canvas;

public static class WebFunc
{
    public static object ToFillStyle(this Color color) => $"rgb({color.R},{color.G},{color.B})";
    public static string ToStrokeStyle(this Color color) => $"rgb({color.R},{color.G},{color.B})";

}
