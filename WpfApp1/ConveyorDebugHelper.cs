using ConveyorLib.Shapes;
using CoreLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using UILib.Shapes;

namespace ConveyorApp
{
    internal class ConveyorDebugHelper : IDebugHelper
    {
        public Point Origin { get; set; } = default;

        List<Shape> Shapes = [];
        public Canvas Canvas { get; set; }
        public IConveyorShapeProvider ShapeProvider { get; internal set; }

        public void PutPoint(Point point)
        {
            var pointShape = ShapeProvider.CreatePoint(point.Add(Origin));
            AddToCanvas(pointShape);
        }

        public void PutVector(Vector vector)
        {
            if (vector.Length() > 2)
            {
                PutLineSegment(((0, 0), vector));
                Vector arrowHeadLeft = (-5, 5);
                var angle = vector.CartPosAngle();
                arrowHeadLeft = arrowHeadLeft.RotateAroundOrigin(angle);
                Vector arrowHeadRight = (-5, -5);
                arrowHeadRight = arrowHeadRight.RotateAroundOrigin(angle);

                PutLineSegment((vector, vector + arrowHeadLeft));
                PutLineSegment((vector, vector + arrowHeadRight));
            }
            else
            {
                PutThinLineSegment(((0, 0), vector));
            }

        }

        public void PutThinLineSegment(TwoPoints fromTo)
        {
            var lineSegment = ShapeProvider.CreateDebugThinLineSegment(fromTo.Add(Origin));
            AddToCanvas(lineSegment);
        }

        public void PutLineSegment(TwoPoints fromTo)
        {
            var lineSegment = ShapeProvider.CreateDebugLineSegment(fromTo.Add(Origin));
            AddToCanvas(lineSegment);
        }

        private void AddToCanvas(IShape shape) => AddToCanvas(shape);

        public void SetOrigin(Point origin) => Origin = origin;

        public void PutAngle(Vector point, Angle angle, Angle rotateBy)
        {
            Vector line1V = (0, 20);
            line1V = line1V.RotateAroundOrigin(rotateBy);
            var line2V = line1V.RotateAroundOrigin(angle);

            var line1 = ShapeProvider.CreateDebugLineSegment((point, point.Add(line1V)));
            var line2 = ShapeProvider.CreateDebugLineSegment((point, point.Add(line2V)));
            AddToCanvas(line1);
            AddToCanvas(line2);
        }
    }
}
