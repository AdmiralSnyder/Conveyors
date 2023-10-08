using System;
using CoreLib;
using InputLib;

namespace ConveyorInputLib.Helpers;

public class CanvasInputHelpers : InputHelpers
{
    public void AddToList<TData>(List<TData> list, TData data) => list.Add(data);

    public TData AddData<TData>(TData data) => data;

    public TData AddData<TData>() where TData : new() => new();

    public ShowPathInputHelper ShowPath(IEnumerable<Point> points)
        => ShowPathInputHelper.Create(Context, points);

    public ShowFixedPointInputHelper ShowFixedPoint(Point point)
        => ShowFixedPointInputHelper.Create(Context, point);

    public ShowCircleByRadiusInputHelper ShowCircleByRadius(Point center)
        => ShowCircleByRadiusInputHelper.Create(Context, center);

    public ShowCircleByDiameterInputHelper ShowCircleByDiameter(Point point1)
        => ShowCircleByDiameterInputHelper.Create(Context, point1);

    public ShowThreePointCircleOnMouseLocationInputHelper ShowThreePointCircleOnMouseLocation(Point point1, Point point2)
        => ShowThreePointCircleOnMouseLocationInputHelper.Create(Context, point1, point2);

    public ShowCalculatedPointInputHelper ShowCalculatedPoint(Func<Point, Point> calcFunction)
        => ShowCalculatedPointInputHelper.Create(Context, calcFunction);

    public ShowMouseLocationInputHelper ShowMouseLocation()
        => ShowMouseLocationInputHelper.Create(Context);

    public ShowLineFromToMouseInputHelper LineFromToMouse(Point point)
        => ShowLineFromToMouseInputHelper.Create(Context, point);

    public WaitForSelectionInputHelper WaitForSelection()
        => WaitForSelectionInputHelper.Create(Context);
}
