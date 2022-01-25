using System;

namespace WpfLib;

public static class MathsFunc
{

    public static Vector Vector(this TwoPoints startEnd) => (startEnd.P2.X - startEnd.P1.X, startEnd.P2.Y - startEnd.P1.Y);

    public static double Length(this Vector vect) => Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y);

    public static double Length(this TwoPoints startEnd) => Length(startEnd.Vector());
    public static Vector Normalize(this Vector vect) => vect.Divide(vect.Length());
    public static Vector Normalize(this Vector vect, double length) => vect.Divide(length);

    public static Vector Multiply(this Vector vect, double factor) => (vect.X * factor, vect.Y * factor);
    public static Vector Divide(this Vector vect, double factor) => (vect.X / factor, vect.Y / factor);
    public static Point Subtract(this Point point, Vector vect) => (point.X - vect.X, point.Y - vect.Y);
    public static Point Add(this Point point, Vector vect) => (point.X + vect.X, point.Y + vect.Y);
    public static TwoPoints Add(this TwoPoints twoPoints, Vector vect) => (twoPoints.P1.Add(vect), twoPoints.P2.Add(vect));
    public static double DotProduct(this Vector a, Vector b) => a.X * b.X + a.Y * b.Y;

#if false
private static (double x, double y) Divide((double x, double y) vect, double divisor) => Multiply(vect, 1 / divisor);
private static (double x, double y) Multiply((double x, double y) vect, double factor) => (x: vect.x * factor, y: vect.y * factor);
private static (double x, double y) Normalize((double x, double y) vect) => Divide(vect, vect.Length());
#endif
    public static double RadToDeg(double rad) => Math.PI / rad; 


    /// <summary>
    /// returns whether the elements in an array have the same distance
    /// </summary>
    /// <param name="arr"></param>
    /// <returns></returns>
    public static bool IsEvenlyDistributed(int[] arr)
    {
        if (arr.Length < 2) return false;
        // 1. step
        var step = arr[1] - arr[0];
        // check whether the step is the same for every consecutive pair in the array
        for(int i = 1; i < arr.Length; i++)
        {
            if (arr[i - 1] + step != arr[i]) return false;
        }
        return true;
    }
}
