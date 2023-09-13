using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILib.Extern.Web.Canvas;

namespace ConveyorLibWeb;

public static class WebFunc
{
    public static Vector GetSizeWeb<TShape>(this TShape shape) where TShape : WebShape => (shape.Width, shape.Height);

    public static WebShape SetLocationWeb(this WebShape shape, Point location)
    {
        shape.Location = location;
        return shape;
    }

    public static WebLine SetLocationWeb(this WebLine line, TwoPoints points)
    {
        line.FromTo = points;
        return line;
    }
}