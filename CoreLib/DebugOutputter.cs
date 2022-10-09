using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public interface IDebugHelper
    {
        public void PutAngle(Point point, Angle angle, Angle rotateBy);

        public void PutPoint(Point point);
        public Point Origin { get; set; }
        public void PutVector(Vector vector);

        public void PutLineSegment(TwoPoints twoPoints);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateImplementationAttribute<T> : Attribute { }

    [GenerateImplementation<IDebugHelper>]
    public static partial class DebugHelper
    {
        public static IDebugHelper? Instance { get; set; }
    }

    [GeneratedCode("ImplementationGenerator", null)]
    partial class DebugHelper
    {
        #region IDebugHelper implementation

        public static void PutPoint(Point point) => Instance?.PutPoint(point);
        public static Point Origin => Instance?.Origin ?? default;
        public static void PutVector(Vector vector) => Instance?.PutVector(vector);
        public static void PutAngle(Point point, Angle angle, Angle rotateBy) => Instance?.PutAngle(point, angle, rotateBy);
        public static void PutAngle(Point point, Angle angle) => Instance?.PutAngle(point, angle, Angle.Zero);

        public static void PutLineSegment(TwoPoints twoPoints) => Instance?.PutLineSegment(twoPoints);
        public static void PutLineSegmentVector(TwoPoints twoPoints)
        {
            if (Instance is null) return;
            var origin = Instance.Origin;
            Instance.Origin = twoPoints.P1;
            Instance.PutVector(twoPoints.P2 - twoPoints.P1);
            Instance.Origin = origin;
        }
        #endregion
    }
}
