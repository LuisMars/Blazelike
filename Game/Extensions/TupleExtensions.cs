namespace Blazelike.Game.Extensions;

public static class TupleExtensions
{
    public static bool Contains(this ((int X, int Y) Start, (int X, int Y) End) edge, (int X, int Y) point)
    {
        var vectorNormal = edge.Start.Vector(edge.End).Normalize();
        var pointNormal = edge.Start.Vector(point).Normalize();
        return vectorNormal.X == pointNormal.X &&
               vectorNormal.Y == pointNormal.Y;
    }

    public static double Distance(this (int X, int Y) tuple, (int X, int Y) other)
    {
        return Math.Sqrt(tuple.DistanceSquared(other));
    }

    public static double Distance(this (double X, double Y) tuple, (double X, double Y) other)
    {
        return Math.Sqrt(tuple.DistanceSquared(other));
    }

    public static double DistanceSquared(this (int X, int Y) tuple, (int X, int Y) other)
    {
        var a = tuple.X - other.X;
        var b = tuple.Y - other.Y;
        return a * a + b * b;
    }

    public static double DistanceSquared(this (double X, double Y) tuple, (double X, double Y) other)
    {
        var a = tuple.X - other.X;
        var b = tuple.Y - other.Y;
        return a * a + b * b;
    }

    public static double Length(this (double X, double Y) tuple)
    {
        return Math.Sqrt(tuple.DistanceSquared((0, 0)));
    }

    public static (double X, double Y) Normalize(this (double X, double Y) tuple)
    {
        var length = tuple.Length();
        return (tuple.X / length, tuple.Y / length);
    }

    public static (double X, double Y) Vector(this (int X, int Y) tuple, (int X, int Y) other)
    {
        return (other.X - tuple.X, other.Y - tuple.Y);
    }
}