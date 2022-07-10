namespace Blazelike.Game.Extensions;

public static class TupleExtensions
{
    public static bool Contains(this ((int X, int Y, int Z) Start, (int X, int Y, int Z) End) edge, (int X, int Y, int Z) point)
    {
        var vectorNormal = edge.Start.Vector(edge.End).Normalize();
        var pointNormal = edge.Start.Vector(point).Normalize();
        return vectorNormal.X == pointNormal.X &&
               vectorNormal.Y == pointNormal.Y &&
               vectorNormal.Z == pointNormal.Z;
    }

    public static double Distance(this (int X, int Y, int Z) tuple, (int X, int Y, int Z) other)
    {
        return Math.Sqrt(tuple.DistanceSquared(other));
    }

    public static double Distance(this (double X, double Y, double Z) tuple, (double X, double Y, double Z) other)
    {
        return Math.Sqrt(tuple.DistanceSquared(other));
    }

    public static double DistanceSquared(this (int X, int Y, int Z) tuple, (int X, int Y, int Z) other)
    {
        var a = tuple.X - other.X;
        var b = tuple.Y - other.Y;
        var c = tuple.Z - other.Z;
        return a * a + b * b + c * c;
    }

    public static double DistanceSquared(this (double X, double Y, double Z) tuple, (double X, double Y, double Z) other)
    {
        var a = tuple.X - other.X;
        var b = tuple.Y - other.Y;
        var c = tuple.Z - other.Z;
        return a * a + b * b + c * c;
    }

    public static double Length(this (double X, double Y, double Z) tuple)
    {
        return Math.Sqrt(tuple.DistanceSquared((0, 0, 0)));
    }

    public static (double X, double Y, double Z) Normalize(this (double X, double Y, double Z) tuple)
    {
        var length = tuple.Length();
        return (tuple.X / length, tuple.Y / length, tuple.Z / length);
    }

    public static (double X, double Y, double Z) Vector(this (int X, int Y, int Z) tuple, (int X, int Y, int Z) other)
    {
        return (other.X - tuple.X, other.Y - tuple.Y, other.Z - tuple.Z);
    }
}