﻿using System;
using InputLib;

namespace ConveyorApp.Inputters.Helpers;

public class CanvasInputHelpers : InputHelpers
{
    public ShowFixedPointInputHelper FixedPoint(Point point)
        => ShowFixedPointInputHelper.Create(Context, point);

    public ShowMouseLocationInputHelper ShowMouseLocation()
        => ShowMouseLocationInputHelper.Create(Context);

    public ShowCircleByRadiusInputHelper ShowCircleByRadius(Point center)
        => ShowCircleByRadiusInputHelper.Create(Context, center);

    public ShowCircleByDiameterInputHelper ShowCircleByDiameter(Point point1)
        => ShowCircleByDiameterInputHelper.Create(Context, point1);

    public ShowThreePointCircleOnMouseLocationInputHelper ShowThreePointCircleOnMouseLocation(Point point1, Point point2)
        => ShowThreePointCircleOnMouseLocationInputHelper.Create(Context, point1, point2);

    public ShowCalculatedPointInputHelper ShowCalculatedPoint(Func<Point, Point> calcFunction)
        => ShowCalculatedPointInputHelper.Create(Context, calcFunction);

    public ShowLineFromToMouseInputHelper LineFromToMouse(Point point)
        => ShowLineFromToMouseInputHelper.Create(Context, point);

    public ShowPickedSelectableInputHelper ShowPickedSelectable(ISelectObject selectable)
        => ShowPickedSelectableInputHelper.Create((WpfCanvasInputContext)Context, selectable);
}
