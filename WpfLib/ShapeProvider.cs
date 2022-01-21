using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

using PointAlias = System.Windows.Point;


namespace PointDef
{
    public struct V2d
    {
        public V2d(double x, double y) => (X, Y) = (x, y);
        public V2d((double x, double y) tuple) => (X, Y) = tuple;
        public double X { get; set; }
        public double Y { get; set; }
        public static implicit operator (double, double)(V2d vect) => (vect.X, vect.Y);
        public static implicit operator Point(V2d vect) => new(vect.X, vect.Y);

        public static implicit operator V2d((double, double) tuple) => new(tuple);
        public static implicit operator V2d(Point p) => new(p.X, p.Y);
    }


    namespace twopoints
    {
        public struct TwoPoints<TVect>
        {
            public TwoPoints(TVect p1, TVect p2) => (P1, P2) = (p1, p2);
            public TwoPoints((TVect x, TVect y) tuple) => (P1, P2) = tuple;

            public TVect P1 { get; set; }
            public TVect P2 { get; set; }

            public static implicit operator TwoPoints<TVect>((TVect, TVect) tuple) => new(tuple);
        }
    }
}
namespace WpfLib
{
    public class ShapeProvider
    {
        protected Line CreateLine(TwoPoints points) => new Line()
        {
            X1 = points.P1.X,
            Y1 = points.P1.Y,
            X2 = points.P2.X,
            Y2 = points.P2.Y,
        };
    }
}
