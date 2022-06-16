using System.Diagnostics;

using PointAlias = System.Windows.Point;


namespace PointDef
{
    [DebuggerDisplay($"{nameof(V2d)} ({nameof(V2d.X)}={{{nameof(V2d.X)}}}, {nameof(V2d.Y)}={{{nameof(V2d.Y)}}})")]
    public struct V2d
    {
        public V2d(double x, double y) => (X, Y) = (x, y);
        public V2d((double x, double y) tuple) => (X, Y) = tuple;

        public static readonly V2d MinValue = (double.MinValue, double.MinValue);
        public static readonly V2d MaxValue = (double.MaxValue, double.MaxValue);

        public double X { get; set; }
        public double Y { get; set; }
        public static implicit operator (double, double)(V2d vect) => (vect.X, vect.Y);
        public static implicit operator PointAlias(V2d vect) => new(vect.X, vect.Y);

        public static implicit operator V2d((double, double) tuple) => new(tuple);
        public static implicit operator V2d(PointAlias p) => new(p.X, p.Y);

        public static V2d operator - (V2d a, V2d b) => new(a.X - b.X, a.Y - b.Y);
        public static V2d operator + (V2d a, V2d b) => new(a.X + b.X, a.Y + b.Y);

        public static bool operator ==(V2d a, V2d b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(V2d a, V2d b) => a.X != b.X || a.Y != b.Y;

        public override string ToString() => $"({X}, {Y})";

        public override bool Equals(object? obj) => obj is V2d objV2d && objV2d.X == X && objV2d.Y == Y;

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
