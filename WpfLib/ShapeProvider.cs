using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace WpfLib
{
    public class ShapeProvider
    {
        public Action<Shape> SelectBehaviour { get; set; }

        protected Line CreateLine(TwoPoints points)
        {
            var line = new Line();
            line.SetLocation(points);
            
            return WithSelectBehaviour(line);
        }

        protected T WithSelectBehaviour<T>(T shape) where T : Shape
        {
            shape.ApplyMouseBehaviour(SelectBehaviour);
            return shape;
        }
    }
}
