using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using WpfLib;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    public class ConveyorShapeProvider : ShapeProvider
    {
        public Line CreateConveyorPositioningLine(TwoPoints points)
        {
            var line = CreateLine(points);
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 2;
            return line;
        }

        public Line CreateConveyorSegmentLine(TwoPoints points)
        {
            var line = CreateLine(points);
            line.Stroke = Brushes.Red;
            line.StrokeThickness = 2;
            return line;
        }

        public Line CreateConveyorSegmentLaneLine(TwoPoints points)
        {
            var line = CreateLine(points);
            line.Stroke = Brushes.White;
            line.StrokeThickness = 1;
            return line;
        }

        public Ellipse CreateConveyorPointEllipse(Point point, bool isFirst, bool isLast)
        {
            const double Size = 4d;
            var result = new Ellipse()
            {
                Width = Size,
                Height = Size,
                Fill = isLast ? Brushes.Red : isFirst ? Brushes.Cyan : Brushes.Blue,
            };
            result.ApplyMouseBehaviour(this.SelectBehaviour);
            result.SetCenterLocation(point);
            return result;
        }

        public Path CreateConveyorPointPath()
        {
            return null;
        }

        public Line CreateConveyorPointLine()
        {
            return null;
        }

        internal Ellipse CreatePointMoveCircle(Point location, Action<Shape> leftClickAction)
        {
            const double Size = 15d;
            Ellipse result = new()
            {
                Width = Size,
                Height = Size,
                Stroke = Brushes.BurlyWood,
                StrokeThickness = 3,
                Fill = Brushes.Transparent,
            };
            result.Cursor = Cursors.Hand;

            result.ApplyMouseBehaviour(leftClickAction, MouseAction.LeftClick);
            result.SetCenterLocation(location);
            return result;
        }

        //private static Result_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    CursorChanger
        //}
    }
}
