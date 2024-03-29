﻿using ConveyorInputLib.Helpers;
using CoreLib;
using InputLib;
using UILib;

namespace ConveyorInputLib.Helpers;

public class ShowFixedPointInputHelper : ShowPointInputHelper<ShowFixedPointInputHelper>
{
    private Point _Location;
    public Point Location
    {
        get => _Location;
        set => Func.Setter(ref _Location, value, () => TmpShape.SetCenterLocation(Location));
    }

    public static ShowFixedPointInputHelper Create(InputContextBase context, Point location)
    {
        var result = Create(context);
        result.Location = location;
        return result;
    }
}
