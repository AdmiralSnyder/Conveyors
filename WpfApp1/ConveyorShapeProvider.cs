using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using WpfLib;
using System.Windows.Media;

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

        public Line CreateConveyorSegmentLine()
        {
            return null;
        }

        public Line CreateConveyorSegmentLaneLine()
        {
            return null;
        }

        public Ellipse CreateConveyorPointEllipse()
        {
            return null;
        }

        public Path CreateConveyorPointPath()
        {
            return null;
        }

        public Line CreateConveyorPointLine()
        {
            return null;
        }
    }
}
