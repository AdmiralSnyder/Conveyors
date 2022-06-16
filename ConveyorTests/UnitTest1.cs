using System.Numerics;
using CoreLib.Maths;
using PointDef;
using PointDef.twopoints;

namespace ConveyorTests
{

    public class UnitTest1
    {
        [Theory()]
        [InlineData(10, 10, 20, 20, 30, 30, 180)]
        [InlineData(10, 10, 20, 20, 30, 20, 135)]
        
        //[InlineData(10, 10, 20, 20, 20, 30, 225)]
        [InlineData(10, 10, 20, 20, 20, 30, -135)]
        public void Test1(double ax, double ay, double bx, double by, double cx, double cy, double angleRad)
        {
            V2d a = new(ax, ay);
            V2d b = new(bx, by);
            V2d c = new(cx, cy);

            var ab = Maths.Vector((b, a)).Normalize();
            var bc = Maths.Vector((b, c)).Normalize();

            var angle = calculateAngleSODeg(ab.X, ab.Y, bc.X, bc.Y);
            Assert.Equal(angleRad, angle);
        }

        [Fact]
        public void Test2()
        {
            Assert.Equal(2, 2);
        }


        /// <summary>
        /// https://stackoverflow.com/questions/13458992/angle-between-two-vectors-2d
        /// </summary>
        public double calculateAngleSODeg(double ax, double ay, double bx, double by)
{
            double sin = ax * by - bx * ay;
            double cos = ax * bx + ay * by;
            return Maths.RadToDeg(Math.Atan2(sin, cos));
        }
    }
}