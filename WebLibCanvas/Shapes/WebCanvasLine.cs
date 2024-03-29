﻿using System.Drawing;
using UILib.Extern.Web.Canvas;
using UILib.Shapes;

namespace ConveyorLibWeb.Shapes;
public class WebCanvasLine : WebCanvasShape<WebLine>, ILine
{
    public WebCanvasLine() : base(new()) { }

    public double X1
    {
        get => BackingObject.FromTo.P1.X;
        set => BackingObject.FromTo = ((value, BackingObject.FromTo.P1.Y), BackingObject.FromTo.P2);
    }

    public double Y1
    {
        get => BackingObject.FromTo.P1.Y;
        set => BackingObject.FromTo = ((BackingObject.FromTo.P1.X, value), BackingObject.FromTo.P2);
    }

    public double X2
    {
        get => BackingObject.FromTo.P2.X;
        set => BackingObject.FromTo = (BackingObject.FromTo.P1, (value, BackingObject.FromTo.P2.Y));
    }

    public double Y2
    {
        get => BackingObject.FromTo.P2.Y;
        set => BackingObject.FromTo = (BackingObject.FromTo.P1, (BackingObject.FromTo.P2.X, value));
    }
}
