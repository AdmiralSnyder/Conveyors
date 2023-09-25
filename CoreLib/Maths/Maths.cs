using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using CoreLib.Definition;
using PointDef;
using PointDef.twopoints;

namespace CoreLib.Maths;

public static class AngleExtensions
{
    public static Angle Degrees(this double valueDeg) => new() { Degrees = valueDeg, Radians = Maths.DegToRad(valueDeg) };
    public static Angle Radians(this double valueRad) => new() { Degrees = Maths.RadToDeg(valueRad), Radians = valueRad };
}

public static class LineDefinitionExtensions
{
    public static bool GetCrossingPoint(this LineDefinition line1, LineDefinition line2, out Point crossingPoint)
    => Maths.GetCrossingPoint(line1.RefPoints, line2.RefPoints, out crossingPoint);
    public static Point GetClosestPointOnLine(this LineDefinition line, Point point)
    {
        if (line.ContainsPoint(point)) return point;

        var orthoVect = line.Vector.Orthogonal();
        LineDefinition orthoLine = new(point, orthoVect);

        if (line.GetCrossingPoint(orthoLine, out var crossingPoint))
        {
            return crossingPoint;
        }
        else throw new MathsException($"{nameof(GetCrossingPoint)} should have returned true");
    }
    public static bool IsLeftOfLine(this LineDefinition line, Point point)
    {
        var closestPoint = line.GetClosestPointOnLine(point);
        var pointVect = closestPoint.To(point);
        //DebugHelper.PutLineSegmentVector((closestPoint, point));
        return Maths.PosAngleBetween(line.Vector, pointVect) < Angle.HalfCircle;
    }

    public static bool ContainsPoint(this LineDefinition line, Point point) => line.IsVertical
        ? point.X == line.ReferencePoint1.X
        : point.Y == line.Slope * point.X + line.OffsetY;
}


public static class Maths
{
    public static Angle AngleBetween(double ax, double ay, double bx, double by, Vector midVector)
    {
        //var angleAB = AngleBetween(ax, ay, bx, by);
        var angle1 = new Vector(ax, ay).CartPosAngle();
        var angle2 = new Vector(bx, by).CartPosAngle();
        Angle negPosAngle1;
        var midAngle = midVector.CartPosAngle();
        if (angle1 > angle2)
        {
            negPosAngle1 = (-((Angle.FullCircle - angle1).Radians)).Radians();
            if (midAngle > angle2)
            {
                midAngle = (-((Angle.FullCircle - midAngle).Radians)).Radians();
            }
        }
        else
        {
            negPosAngle1 = angle1;
        }

        if (midAngle >= negPosAngle1 && midAngle <= angle2)
        {
            return PosAngleBetween(ax, ay, bx, by);
        }
        else
        {
            return Angle.FullCircle - PosAngleBetween(ax, ay, bx, by);
        }

    }

    //public static Angle PosAngleBetween(double ax, double ay, double bx, double by, Vector midVector)
    //{
    //    var aAngle = CartPosAngle((ax, ay));
    //    var bAngle = CartPosAngle((bx, by));
    //    var midAngle = CartPosAngle(midVector);

    //    if (aAngle == bAngle) return 0d.Degrees();

    //    if (aAngle > bAngle) return (360d.Degrees() - aAngle) + bAngle;

    //    return bAngle - aAngle;
    //}

    public static Angle CartAngle(this Vector vector) => AngleBetween(1, 0, vector.X, vector.Y);
    public static Angle CartPosAngle(this Vector vector) => PosAngleBetween(1, 0, vector.X, vector.Y);

    public static Angle AngleBetween(double ax, double ay, double bx, double by)
    {
        double sin = ax * by - bx * ay;
        double cos = ax * bx + ay * by;
        return Math.Atan2(sin, cos).Radians();
    }


    public static Angle PosAngleBetween(double ax, double ay, double bx, double by)
    {
        double sin = ax * by - bx * ay;
        double cos = ax * bx + ay * by;
        var rad = Math.Atan2(sin, cos);
        if (rad < 0)
        {
            rad = Math.Tau + rad;
        }
        return rad.Radians();
    }

    public static Angle AngleBetween(Vector a, Vector b, Vector midVector) => AngleBetween(a.X, a.Y, b.X, b.Y, midVector);

    public static Angle AngleBetween(Vector a, Vector b) => AngleBetween(a.X, a.Y, b.X, b.Y);

    //public static Angle PosAngleBetween(Vector a, Vector b, Vector midVector) => PosAngleBetween(a.X, a.Y, b.X, b.Y, midVector);

    public static Angle PosAngleBetween(Vector a, Vector b) => PosAngleBetween(a.X, a.Y, b.X, b.Y);

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

    public static Vector Halve(this Vector vect) => vect.Divide(2);
    public static Vector Multiply(this Vector vect, double factor) => (vect.X * factor, vect.Y * factor);
    public static Vector Divide(this Vector vect, double divisor) => (vect.X / divisor, vect.Y / divisor);
    public static Point Subtract(this Point point, Vector vect) => (point.X - vect.X, point.Y - vect.Y);
    public static Point Add(this Point point, Vector vect) => (point.X + vect.X, point.Y + vect.Y);
    public static TwoPoints Add(this TwoPoints twoPoints, Vector vect) => (twoPoints.P1.Add(vect), twoPoints.P2.Add(vect));
    public static TwoPoints Subtract(this TwoPoints twoPoints, Vector vect) => (twoPoints.P1.Subtract(vect), twoPoints.P2.Subtract(vect));

    public static IEnumerable<Point> Add(this IEnumerable<Point> points, Vector vect) => points.Select(p => p.Add(vect));
    public static IEnumerable<Point> Scale(this IEnumerable<Point> points, double factor) => points.Select(p => p.Multiply(factor));

    public static bool IsSelectionMatch(this Point point, Point selectCoords) => (point - selectCoords).Length() <= 2;
    public static bool IsSelectionMatch(this CircleDefinition circle, Point selectCoords) => (circle.CenterRadius.Center - selectCoords).Length() <= 2 + circle.CenterRadius.Radius;

    public static bool IsSelectionMatch(this LineDefinition line, Point selectCoords)
    {
        var closestPoint = line.GetClosestPointOnLine(selectCoords);
        return (closestPoint - selectCoords).Length() <= 2;
    }

    public static bool IsSelectionMatch(this PointObjDefinition pointDefinition, Point selectCoords) 
        => pointDefinition.Point.IsSelectionMatch(selectCoords);

    public static bool IsSelectionMatchSegment(LineDefinition segmentDefinition, Point selectCoords)
    {
        if (segmentDefinition.ReferencePoint1.IsSelectionMatch(selectCoords)) return true;
        if (segmentDefinition.ReferencePoint2.IsSelectionMatch(selectCoords)) return true;

        var closestPoint = segmentDefinition.GetClosestPointOnLine(selectCoords);
        if (!closestPoint.IsSelectionMatch(selectCoords)) return false;
        return closestPoint.IsInBounds(segmentDefinition.RefPoints.AsBounds());
    }

    public static Bounds AsBounds(this TwoPoints points) 
        => new((points.P1.X.Min(points.P2.X), points.P1.Y.Min(points.P2.Y)), (Math.Abs(points.P1.X - points.P2.X), Math.Abs(points.P1.Y - points.P2.Y)));

    public static double Min(this double value, double otherValue) => Math.Min(value, otherValue);
    public static bool IsPointOnLineSegment(Point p, Point start, Point end)
{
    // Set up internal values
    var minX = Math.Min(start.X, end.X);
    var maxX = Math.Max(start.X, end.X);
    var minY = Math.Min(start.Y, end.Y);
    var maxY = Math.Max(start.Y, end.Y);

    // Check if the point is within the bounds of the line segment
    return p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY && (p.X - start.X) * (end.Y - start.Y) == (end.X - start.X) * (p.Y - start.Y);
}

    public static double DotProduct(this Vector a, Vector b) => a.X * b.X + a.Y * b.Y;

    public static double Determinant(this Vector a, Vector b) => a.X * b.X - a.Y * b.Y;

    public static double CrossProduct(this Vector a, Vector b) => a.X * b.Y - a.Y * b.X;

    public static Point RotateAround(this Point rotPoint, Point origin, Angle angle)
    {
        var originatePoint = rotPoint - origin;

        var originatedRotPoint = originatePoint.RotateAroundOrigin(angle);
        return originatedRotPoint + origin;
    }

    public static Point RotateAroundOrigin(this Point point, Angle angle)
    {
        angle = angle; // fix for (flipped y axis.
        var r = point.Length();
        var firstAnglePointInfos = BringInFirstQuadrant(point);
        var firstAngle = firstAnglePointInfos.FirstQuadrantPoint.Angle() + firstAnglePointInfos.CorrectionAngle;
        var totalAngle = firstAngle + angle;

        var x2 = Math.Round(Math.Cos(totalAngle.Radians) * r, 7);
        var y2 = Math.Round(Math.Sin(totalAngle.Radians) * r, 7);

        return (x2, y2);
    }

    public static (Point FirstQuadrantPoint, Angle CorrectionAngle) BringInFirstQuadrant(Point point) => point.Quadrant switch
    {
        Quadrants.One => (point, Angle.Zero),
        Quadrants.Two => ((point.Y, -point.X), Angle.Plus90),
        Quadrants.Three => (point.Inverse(), Angle.Plus180),
        Quadrants.Four => ((-point.Y, point.X), Angle.Plus270),
        _ => throw new NotImplementedException(),
    };

    public static Point GetMidPoint(Point point1, Point point2) => point1 + (point2 - point1).Divide(2);

    public static Bounds GetBounds(TwoPoints twoPoints)
    {
        var x = Math.Min(twoPoints.P1.X, twoPoints.P2.X);
        var y = Math.Min(twoPoints.P1.Y, twoPoints.P2.Y);

        var width = Math.Max(twoPoints.P1.X, twoPoints.P2.X) - x;
        var height = Math.Max(twoPoints.P1.Y, twoPoints.P2.Y) - y;
        return new((x, y), (width, height));
    }

    public enum Quadrants
    {
        One,
        Two,
        Three,
        Four
    }

    public static Point RotateAroundOld(this Point rotPoint, Point origin, double angle)
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

    public static Bounds GetBoundingRectTopLeftSize(IEnumerable<Point> points)
    {
        var pointsArr = points.ToArray();
        return pointsArr.Length switch
        {
            0 => throw new NotImplementedException("case for 0 points missing"),
            1 => new(pointsArr[0], default),
            _ => new(GetTopLeft(pointsArr), GetSize(pointsArr)),
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

    public static bool VectorsAreParallel(Vector v1, Vector v2) => v1.Angle() == v2.Angle();

    public static bool VectorsAreInverseParallel(Vector v1, Vector v2) => ((v1.Angle() - v2.Angle()).Degrees / 180.0) is { } offset && double.IsOddInteger(offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool False<T>(out T parameter)
    {
        parameter = default!;
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool False<T1, T2>(out T1 parameter1, out T2 parameter2)
    {
        parameter1 = default!;
        parameter2 = default!;
        return false;
    }

    public static bool GetCircleInfoByCenterRadius((Vector Center, double Radius) centerRadius, out (Vector point1, Vector point2, Vector point3) threePoints)
    {
        if (centerRadius.Radius <= 0) return False(out threePoints);

        threePoints = (
            centerRadius.Center.Add((centerRadius.Radius, 0)),
            centerRadius.Center.Add((-centerRadius.Radius, 0)),
            centerRadius.Center.Add((0, centerRadius.Radius))
            );
        return true;
    }

    public static bool GetCircleInfoByDiameter((Vector Point1, Vector Point2) info, out (Vector Center, double Radius) circInfo, out Point thirdPoint)
    {
        if (info.Point1 == info.Point2) return False(out circInfo, out thirdPoint);

        var radiusV = (info.Point2 - info.Point1).Divide(2);
        var radius = radiusV.Length();
        var center = info.Point1 + radiusV;
        circInfo = (center, radius);
        return CalculateCircleThirdPoint(info.Point1, info.Point2, center, radius, out thirdPoint);

    }

    public static bool CalculateCircleThirdPoint(Vector point1, Vector point2, Vector center, double radius, out Vector thirdPoint)
    {
        if (radius <= 0)
        {
            thirdPoint = default;
            return false;
        }
        thirdPoint = center.Add((radius, 0));
        if (thirdPoint == point1)
        {
            thirdPoint = center.Add((-radius, 0));
            if (thirdPoint == point2)
            {
                thirdPoint = center.Add((0, radius));
            }
        }
        else if (thirdPoint == point2)
        {
            thirdPoint = center.Add((-radius, 0));
            if (thirdPoint == point1)
            {
                thirdPoint = center.Add((0, radius));
            }
        }
        return true;
    }

    public static bool GetCircleInfoByThreePoints((Vector Point1, Vector Point2, Vector Point3) info, out (Vector Center, double Radius) circInfo)
    {
        var x1 = info.Point1.X;
        var x2 = info.Point2.X;
        var x3 = info.Point3.X;
        var y1 = info.Point1.Y;
        var y2 = info.Point2.Y;
        var y3 = info.Point3.Y;

        var z1 = x1 * x1 + y1 * y1;
        var z2 = x2 * x2 + y2 * y2;
        var z3 = x3 * x3 + y3 * y3;

        var A = Determinant(1, 1, 1, y1, y2, y3, z1, z2, z3);
        var B = Determinant(x1, x2, x3, 1, 1, 1, z1, z2, z3);
        var C = Determinant(x1, x2, x3, y1, y2, y3, 1, 1, 1);
        var D = Determinant(x1, x2, x3, y1, y2, y3, z1, z2, z3);

        if (C == 0)
        {
            circInfo = default;
            return false;
        }

        var xC = -A / (2 * C);
        var yC = -B / (2 * C);

        var rSquared = (A * A + B * B + 4 * C * D) / (4 * C * C);
        var r = Math.Sqrt(rSquared);

        circInfo = ((xC, yC), r);
        return true;
    }

    public static double Determinant(
        double a, double b, double c,
        double d, double e, double f,
        double g, double h, double i)
    => a * Determinant(e, f, h, i) - b * Determinant(d, f, g, i) + c * Determinant(d, e, g, h);

    public static double Determinant(
        double a, double b,
        double c, double d)
    => a * d - b * c;

    public static bool GetSlope(this Vector vector, out double slope)
    {
        if (vector.X == 0)
        {
            slope = double.NaN;
            return false;
        }
        else
        {
            slope = vector.Y / vector.X;
            return true;
        }
    }

    public static Vector Orthogonal(this Vector vector) => vector.RotateAroundOrigin(Angle.Plus90);

    /// <summary> y2 - m * x2 </summary>
    public static double GetOffsetY(Point endPoint, double slope) => endPoint.Y - slope * endPoint.X;

    public static bool GetOffsetY(TwoPoints points, out double offsetY)
    {
        if (GetSlope(points.P2 - points.P1, out var slope))
        {
            offsetY = points.P2.Y - slope * points.P2.X;
            return true;
        }
        else
        {
            offsetY = double.NaN;
            return false;
        }
    }


    public static bool GetCrossingPoint(TwoPoints line1, TwoPoints line2, out Point crossingPoint)
    {
        var R1 = line2.P1;
        var R2 = line2.P2;
        var P1 = line1.P1;
        var P2 = line1.P2;

        var yr1 = R1.Y;
        var xp = P2.X - P1.X;
        var yp1 = P1.Y;
        var xr1 = R1.X;
        var yp = P2.Y - P1.Y;
        var xp1 = P1.X;
        var xr = R2.X - R1.X;
        var yr = R2.Y - R1.Y;
        var quotient = (xr * yp - yr * xp);
        if (quotient == 0)
        {
            crossingPoint = default;
            return false;
        }
        else
        {
            var sr = (yr1 * xp - yp1 * xp - xr1 * yp + xp1 * yp) / quotient; // TODO what happens if zero??

            var xq = xr1 + sr * (R2.X - R1.X);
            var yq = yr1 + sr * (R2.Y - R1.Y);

            var cross = new Vector(xq, yq);
            var start = R1;
            var end = P2;
            //crossStart = start - cross;
            //crossEnd = end - cross;

            crossingPoint = cross;
            return true;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <image url="../docs/FilletOrientation.png"></image>
    /// <param name="line1"></param>
    /// <param name="line2"></param>
    /// <param name="selectionPoints"></param>
    /// <param name="filletInfo"></param>
    /// <returns></returns>
    public static bool CreateFilletInfo(LineDefinition line1, LineDefinition line2, TwoPoints selectionPoints, out (TwoPoints Points, double Radius) filletInfo)
    {
        // repro for wrong angle 
        // $.AddLine(((110, 60), (120, 80)));
        // $.AddLine(((70, 60), (100, 80)))


        var selectionMidPoint = selectionPoints.MidPoint();
        //DebugHelper.PutPoint(selectionMidPoint);
        //DebugHelper.PutPoint(selectionPoints.P1);
        //DebugHelper.PutPoint(selectionPoints.P2);

        // TODO decide which angle to pick from param
        if (line1.GetCrossingPoint(line2, out var crossingPoint))
        {

            var unitVector1 = line1.Vector.Normalize();
            var closestPoint1 = line1.GetClosestPointOnLine(selectionPoints.P1);
            unitVector1 = Maths.OrientVectorTowards(unitVector1, (crossingPoint, closestPoint1));

            //DebugHelper.PutLineSegmentVector((crossingPoint, crossingPoint + unitVector1.Multiply(40)));

            var unitVector2 = line2.Vector.Normalize();
            var closestPoint2 = line2.GetClosestPointOnLine(selectionPoints.P2);
            unitVector2 = Maths.OrientVectorTowards(unitVector2, (crossingPoint, closestPoint2));

            var closestPointMidPoint = MidPoint((closestPoint1, closestPoint2));
            //DebugHelper.PutLineSegmentVector((crossingPoint, crossingPoint + unitVector2.Multiply(40)));

            //DebugHelper.PutLineSegmentVector((crossingPoint, closestPointMidPoint));

            var angle = Maths.AngleBetween(unitVector1, unitVector2, crossingPoint.To(closestPointMidPoint).Normalize());
            //Debug.WriteLine(angle);

            //DebugHelper.PutAngle((20, 20), angle);

            // TODO radius needs to be a param
            var radius = 25d;
            var tangent = Math.Tan(angle.CounterAngle().Radians / 2);

            var a = tangent * radius;

            var start1 = crossingPoint + unitVector1.Multiply(a);
            var start2 = crossingPoint + unitVector2.Multiply(a);

            //DebugHelper.PutPoint(start1);
            //DebugHelper.PutPoint(start2);

            TwoPoints startPoints = (start1, start2);
            LineDefinition filletVect = new((startPoints.MidPoint(), crossingPoint));

            //DebugHelper.PutLineSegmentVector(filletVect.RefPoints);

            if (filletVect.IsLeftOfLine(start1))
            {
                filletInfo = ((start2, start1), radius);
            }
            else
            {
                filletInfo = ((start1, start2), radius);
            }

            return true;
        }
        else
        {
            filletInfo = default;
            return false;
        }
    }


    public static Point MidPoint(this TwoPoints points) => MidPoint(points.P1, points.P2);

    public static Point MidPoint(Point p1, Point p2) => p1.Add(p1.To(p2).Halve());

    public static Vector OrientVectorTowards(Vector vector, TwoPoints fromTo)
    {
        var pAdd = fromTo.P1 + vector;
        var pSub = fromTo.P1 - vector;
        if ((fromTo.P2 - pAdd).Length() < (fromTo.P2 - pSub).Length())
        {
            return vector;
        }
        else
        {
            return vector.Inverse();
        }
    }

    public static bool PointsAreEqual(this TwoPoints twoPoints) => twoPoints.P1 == twoPoints.P2;

    public static bool IsParallel(this Vector v1, Vector v2)
    {
        if (v1.X == 0 && v2.X == 0 || v1.Y == 0 && v2.Y == 0) return true;

        var k = v1.X / v2.X;
        return v1.Y / k == v2.Y;
    }

    public static bool LineCrossesPoint(this LineDefinition line, Point point)
        => new Vector(line.ReferencePoint1, point).IsParallel(line.Vector);

    public static Point[] LineCrossesLine(LineDefinition line1, LineDefinition line2)
    {
        if (line1.Vector.IsParallel(line2.Vector))
        {
            if (line1.LineCrossesPoint(line2.ReferencePoint1))
            {
                return new Point[2];
            }
            else
            {
                return Array.Empty<Point>();
            }
        }
        else
        {
            if (GetCrossingPoint(line1.RefPoints, line2.RefPoints, out var cp))
            {
                return new[] { cp };
            }
            else throw new MathsException("previous checks were invalid");
        }
    }

    public enum LineCircleCrossTypes
    {
        Error = -1, // is this bad?
        NoPoint = 0,
        OnePoint = 1,
        TwoPoints = 2,
    }

    public static LineCircleCrossTypes CircleCrossesLine(CircleDefinition circle, LineDefinition line, out Point[] result)
    {
        result = default;

        if (circle.IsDegenerated) return LineCircleCrossTypes.Error;
        var (c, r) = circle.CenterRadius;

        if (LineCrossesPoint(line, c))
        {
            result = [
                c.Add(line.Vector.Normalize().Multiply(r)),
                c.Add(line.Vector.Normalize().Multiply(-r)),
            ];
            return LineCircleCrossTypes.TwoPoints;
        }

        LineDefinition normalThroughCenter = new(c, line.Vector.Orthogonal());
        var lineCrossPoint = LineCrossesLine(line, normalThroughCenter)[0];
        var distance = new Vector(c, lineCrossPoint).Length();
        if (distance == r) // TODO rounding, maybe?
        {
            result = [lineCrossPoint];
            return LineCircleCrossTypes.OnePoint;
        }
        else if (distance < r)
        {
            Pythagoras(distance, out var kath, r);
            result = [
                lineCrossPoint.Add(line.Vector.Normalize().Multiply(kath)),
                lineCrossPoint.Add(line.Vector.Normalize().Multiply(-kath))
            ];
            return LineCircleCrossTypes.TwoPoints;
        }
        else
        {
            return LineCircleCrossTypes.NoPoint;
        }
    }

    public static void Pythagoras(double a, double b, out double c)
    {
        c = Math.Sqrt(a * a + b * b);
    }

    public static void Pythagoras(double a, out double b, double c)
    {
        // a^2+b^2=c^2
        // b^2=c^2-a^2
        // b=sqrt(c^2-a^2)
        b = Math.Sqrt(c * c - a * a);
    }

    public static bool PointIsInCircle(Point point, CircleDefinition definition)
    {
        var (c, r) = definition.CenterRadius;
        return new Vector(point, c).Length() < r;
    }

    public static bool IsInInterval(this double value, double minIncluding, double maxIncluding) 
        => minIncluding <= value && value <= maxIncluding;
    public static bool IsInInterval(this double value, Interval interval) => IsInInterval(value, interval.Min, interval.Max);
    public static bool IsInIntervalExcluding(this double value, double minExcluding, double maxExcluding) 
        => minExcluding < value && value < maxExcluding;

    public static bool IsInIntervalIncludingExcluding(this double value, double minIncluding, double maxExcluding) 
        => minIncluding <= value && value < maxExcluding;
    public static bool IsInIntervalExcludingIncluding(this double value, double minExcluding, double maxIncluding) 
        => minExcluding < value && value <= maxIncluding;

    public static bool IsInBounds(this Point point, Bounds bounds) 
        => point.X.IsInInterval(bounds.Location.X, bounds.Location.X + bounds.Size.X) 
        && point.Y.IsInInterval(bounds.Location.Y, bounds.Location.Y + bounds.Size.Y);

    public static bool IsInBounds(this Point point, Point boundsPoint1, Point boundsPoint2) => point.IsInBounds(new Bounds(boundsPoint1, (boundsPoint2 - boundsPoint1)));
    public static bool Contains(this Bounds bounds, Point point) => point.IsInBounds(bounds);

    public static bool Contains(this Bounds bounds, Bounds otherBounds)
        => otherBounds.Location.IsInBounds(bounds) && otherBounds.BottomRight().IsInBounds(bounds);

    public static bool Intersects(this Bounds bounds, Bounds otherBounds)
        => bounds.IntersectsX(otherBounds) && bounds.IntersectsY(otherBounds);

    public static bool IntersectsX(this Bounds bounds, Bounds otherBounds) => bounds.IntervalX().Intersects(otherBounds.IntervalX());
    public static bool IntersectsY(this Bounds bounds, Bounds otherBounds) => bounds.IntervalY().Intersects(otherBounds.IntervalY());

    public static Point BottomRight(this Bounds bounds) => bounds.Location + bounds.Size;
    public static Interval IntervalX(this Bounds bounds) => new(bounds.Location.X, bounds.Location.X + bounds.Size.X);
    public static Interval IntervalY(this Bounds bounds) => new(bounds.Location.Y, bounds.Location.Y + bounds.Size.Y);

    public static bool Intersects(this Interval interval, Interval otherInterval) 
        => interval.Min <= otherInterval.Max && otherInterval.Min <= interval.Max;
    public static bool Contains(this Interval interval, Interval otherInterval) 
        => interval.Min <= otherInterval.Min && otherInterval.Max <= interval.Max;
    public static bool ContainsExcluding(this Interval interval, Interval otherInterval) 
        => interval.Min < otherInterval.Min && otherInterval.Max < interval.Max;

    public static Point Min(this Point point, Point otherPoint) => new(Math.Min(point.X, otherPoint.X), Math.Min(point.Y, otherPoint.Y));
    public static Point Max(this Point point, Point otherPoint) => new(Math.Max(point.X, otherPoint.X), Math.Max(point.Y, otherPoint.Y));

    // TODO maybe change it to branch into dimensions first?
    public static Bounds Union(this Bounds bounds, Bounds otherBounds) 
        => new(bounds.Location.Min(otherBounds.Location), bounds.BottomRight().Max(otherBounds.BottomRight()));
}

public class MathsException : Exception
{
    public MathsException() { }

    public MathsException(string? message) : base(message) { }

    public MathsException(string? message, Exception? innerException) : base(message, innerException) { }

    protected MathsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}