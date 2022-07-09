namespace Blazelike.Game.Extensions;

public static class TupleExtensions
{
    public static double Distance(this (int X, int Y, int Z) tuple, (int X, int Y, int Z) other)
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
}