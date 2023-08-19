using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILib;

namespace ConveyorApp.Inputters
{
    public class InputHelpers { }

    public class InputHelpers<TContext> : InputHelpers
        where TContext : InputContextBase
    {
        public TContext Context { get; set; }

        public ShowUserNotesInputHelper ShowUserNotes(string notes)
            => ShowUserNotesInputHelper.Create(Context, notes);
    }

    
    public class CanvasInputHelpers : InputHelpers<CanvasInputContext>
    {
        public FixedPointInputHelper FixedPoint(Point point) 
            => FixedPointInputHelper.Create(Context, point);

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
            => ShowPickedSelectableInputHelper.Create(Context, selectable);
    }
}
