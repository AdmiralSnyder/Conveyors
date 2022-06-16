using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib.Maths;

public static class AngleExtensions
{
    public static Angle Degrees(this double valueDeg) => new() { Degrees = valueDeg, Radians = Maths.DegToRad(valueDeg) };
    public static Angle Radians(this double valueRad) => new() { Degrees = Maths.RadToDeg(valueRad), Radians = valueRad };
}
public class Angle
{
    public double Degrees { get; init; }
    public double Radians { get; init; }
    public override string ToString() => $"{Degrees:0.##}° {Radians:0.##}rad";
    public bool IsStraight => Degrees == 180d;
    /// <summary>
    /// Spitzer Winkel
    /// </summary>
    public bool IsAcute => Degrees < 90d;
    /// <summary>
    /// Stumpfer Winkel
    /// </summary>
    public bool IsObtuse => Degrees > 90d;
    public bool IsRight => Degrees == 90d;
    public bool IsReflex => Degrees > 180d && Degrees < 360d;
    public bool IsNormalized => Degrees < 360d;
    public bool IsClockwise => Degrees < 0d;
}


public static class Maths
{
    public static Angle AngleBetween(double ax, double ay, double bx, double by)
    {
        double sin = ax * by - bx * ay;
        double cos = ax * bx + ay * by;
        return Math.Atan2(sin, cos).Radians();
    }

    public static Angle AngleBetween(Vector a, Vector b) => AngleBetween(a.X, a.Y, b.X, b.Y);

    public const double OneEightyOverPi = 180d / Math.PI;
    public const double PiOverOneEighty = Math.PI / 180d;

    public static double RadToDeg(double rad) => rad * OneEightyOverPi;

    public static double DegToRad(double deg) => deg * PiOverOneEighty;

    public static double Distance(Point p1, Point p2)
    {
        p1.X = p2.X - p1.X;
        p1.X *= p1.X;
        p1.Y = p2.Y - p1.Y;
        p1.Y *= p1.Y;
        return Math.Sqrt(p1.X + p1.Y);
    }

    public static double Length(this (double x, double y) tuple) => Distance(new(0, 0), new(tuple.x, tuple.y));

    public const double PiHalf = Math.PI / 2;

    public static readonly Vector XAxisV1 = new(1d, 0d);

    public static readonly Vector YAxisV1 = new(0d, 1d);

    public static Vector Vector(this TwoPoints startEnd) => (startEnd.P2.X - startEnd.P1.X, startEnd.P2.Y - startEnd.P1.Y);

    public static Vector To(this Point from, Point to) => new TwoPoints(from, to).Vector();

    public static double Length(this Vector vect) => Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y);

    public static double Length(this TwoPoints startEnd) => Length(startEnd.Vector());
    public static Vector Normalize(this Vector vect) => vect.Divide(vect.Length());
    public static Vector Normalize(this Vector vect, double length) => vect.Divide(length);

    public static Vector Inverse(this Vector vect) => new(-vect.X, -vect.Y);
    public static Vector InverseY(this Vector vect) => new(vect.X, -vect.Y);

    public static Vector Multiply(this Vector vect, double factor) => (vect.X * factor, vect.Y * factor);
    public static Vector Divide(this Vector vect, double factor) => (vect.X / factor, vect.Y / factor);
    public static Point Subtract(this Point point, Vector vect) => (point.X - vect.X, point.Y - vect.Y);
    public static Point Add(this Point point, Vector vect) => (point.X + vect.X, point.Y + vect.Y);
    public static TwoPoints Add(this TwoPoints twoPoints, Vector vect) => (twoPoints.P1.Add(vect), twoPoints.P2.Add(vect));

    public static IEnumerable<Point> Add(this IEnumerable<Point> points, Vector vect) => points.Select(p => p.Add(vect));
    public static IEnumerable<Point> Scale(this IEnumerable<Point> points, double factor) => points.Select(p => p.Multiply(factor));
    public static double DotProduct(this Vector a, Vector b) => a.X * b.X + a.Y * b.Y;

    public static double Determinant(this Vector a, Vector b) => a.X * b.X - a.Y * b.Y;

    public static double CrossProduct(this Vector a, Vector b) => a.X * b.Y - a.Y * b.X;

    public static Point RotateAround(this Point rotPoint, Point origin, double angle)
    {
        // https://stackoverflow.com/a/705474
        // 00 topleft
        // x_rotated = ((x - dx) * cos(angle)) - ((dy - y) * sin(angle)) + dx
        // y_rotated = dy - ((dy - y) * cos(angle)) + ((x - dx) * sin(angle))
        //  var x = ((rotPoint.X - origin.X) * Math.Cos(angle)) - ((origin.Y - rotPoint.Y) * Math.Sin(angle)) + origin.X;
        //  var y = origin.Y - ((origin.Y - rotPoint.Y) * Math.Cos(angle)) + ((rotPoint.X - origin.X) * Math.Sin(angle));
        // 00 botleft
        // x_rotated = ((x - dx) * cos(angle)) - ((y - dy) * sin(angle)) + dx
        // y_rotated = ((x - dx) * sin(angle)) + ((y - dy) * cos(angle)) + dy
        //  var x = ((rotPoint.X - origin.X) * Math.Cos(angle)) - ((rotPoint.Y - origin.Y) * Math.Sin(angle)) + origin.X;
        //  var y = ((rotPoint.X - origin.Y) * Math.Sin(angle)) + ((rotPoint.Y - origin.Y) * Math.Sin(angle)) + origin.Y;

        var x = ((rotPoint.X - origin.X) * Math.Cos(angle)) - ((origin.Y - rotPoint.Y) * Math.Sin(angle)) + origin.X;
        var y = origin.Y - ((origin.Y - rotPoint.Y) * Math.Cos(angle)) + ((rotPoint.X - origin.X) * Math.Sin(angle));

        return (x, y);
    }

    public static Point GetPointOnLine(this TwoPoints twoPoints, double length, bool overshoot = false)
    {
        var vect = twoPoints.Vector();
        var unitVector = vect.Normalize();
        var len = vect.Length();
        return GetPointOnLine(twoPoints, length, unitVector, len, overshoot);
    }

    public static Point GetPointOnLine(this TwoPoints twoPoints, double length, Vector unitVector, double lineLength, bool overshoot = false)
    {
        if (length < lineLength || overshoot)
        {
            var mult = length;// TODO hier ggf. auf spline umstellen
            return new(unitVector.X * mult + twoPoints.P1.X, unitVector.Y * mult + twoPoints.P1.Y);
        }
        else
        {
            return twoPoints.P2;
        }
    }

    public static TwoPoints GetBoundingRectTopLeftSize(IEnumerable<Point> points)
    {
        var pointsArr = points.ToArray();
        return pointsArr.Length switch
        {
            0 => throw new NotImplementedException("case for 0 points missing"),
            1 => (pointsArr[0], default),
            _ => (GetTopLeft(pointsArr), GetSize(pointsArr)),
        };
    }

    public static Point GetTopLeft(Point[] points)
    {
        Point result = Point.MaxValue;
        foreach (var p in points)
        {
            result.X = Math.Min(result.X, p.X);
            result.Y = Math.Min(result.Y, p.Y);
        }
        return result;
    }

    public static Point GetSize(Point[] points)
    {
        Point topLeft = Point.MaxValue;
        Point bottomRight = Point.MinValue;
        foreach (var p in points)
        {
            topLeft.X = Math.Min(topLeft.X, p.X);
            topLeft.Y = Math.Min(topLeft.Y, p.Y);

            bottomRight.X = Math.Max(bottomRight.X, p.X);
            bottomRight.Y = Math.Max(bottomRight.Y, p.Y);
        }
        return bottomRight.Subtract(topLeft);
    }

#if false
private static (double x, double y) Divide((double x, double y) vect, double divisor) => Multiply(vect, 1 / divisor);
private static (double x, double y) Multiply((double x, double y) vect, double factor) => (x: vect.x * factor, y: vect.y * factor);
private static (double x, double y) Normalize((double x, double y) vect) => Divide(vect, vect.Length());
#endif

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
        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i - 1] + step != arr[i]) return false;
        }
        return true;
    }
}
