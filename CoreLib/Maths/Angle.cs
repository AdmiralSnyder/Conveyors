namespace CoreLib.Maths;

public class Angle
{

    public static Angle Zero = 0d.Degrees();
    public static Angle Minus90 = (-90d).Degrees();
    public static Angle Minus180 = (-180d).Degrees();
    public static Angle Minus270 = (-270d).Degrees();

    public static Angle Plus90 = (90d).Degrees();
    public static Angle Plus180 = (180d).Degrees();
    public static Angle Plus270 = (270d).Degrees();

    public static Angle FullCircle = 360d.Degrees();
    public static Angle HalfCircle = 180d.Degrees();
    public double Degrees { get; init; }
    public double Radians { get; init; }
    public override string ToString() => $"{Degrees:0.##}° {Radians:0.##}rad";

    public static Angle operator -(Angle @this) => (-@this.Radians).Radians();
    public static bool operator <(Angle @this, Angle other) => @this.Radians < other.Radians;
    public static bool operator >(Angle @this, Angle other) => @this.Radians > other.Radians;

    public static bool operator <=(Angle @this, Angle other) => @this.Radians <= other.Radians;
    public static bool operator >=(Angle @this, Angle other) => @this.Radians >= other.Radians;

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

    public Angle CounterAngle() => Radians > 0 ? (Math.PI - Radians).Radians() : (-Math.PI - Radians).Radians();

    public static Angle operator +(Angle a1, Angle a2) => (a1.Radians + a2.Radians).Radians();
    public static Angle operator -(Angle a1, Angle a2) => (a1.Radians - a2.Radians).Radians();

    public static Angle operator ~(Angle a1) => (-a1.Radians).Radians();
    public static Angle operator *(Angle a1, double times) => (a1.Radians * times).Radians();

    public static bool operator ==(Angle a1, Angle a2) => a1.Equals(a2);
    public static bool operator !=(Angle a1, Angle a2) => !a1.Equals(a2);

    public override bool Equals(object? obj) => obj is Angle a && a.Degrees == Degrees;

    // TODO maybe improve that HashCode...
    public override int GetHashCode() => HashCode.Combine(typeof(Angle).GetHashCode(), Degrees);
}
