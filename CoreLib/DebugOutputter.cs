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
        public void SetOrigin(Point origin);
        public void PutVector(Vector vector);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateImplementationAttribute<T> : Attribute { }

    [GenerateImplementation<IDebugHelper>]
    public static partial class DebugHelper
    {
        public static IDebugHelper Instance { get; set; }
    }

    [GeneratedCode("ImplementationGenerator", null)]
    partial class DebugHelper
    {
        #region IDebugHelper implementation

        public static void PutPoint(Point point) => Instance?.PutPoint(point);
        public static void SetOrigin(Point origin) => Instance?.SetOrigin(origin);
        public static void PutVector(Vector vector) => Instance?.PutVector(vector);

        public static void PutAngle(Point point, Angle angle, Angle rotateBy) => Instance?.PutAngle(point, angle, rotateBy);

        public static void PutAngle(Point point, Angle angle) => Instance?.PutAngle(point, angle, Angle.Zero);

        #endregion
    }
}
