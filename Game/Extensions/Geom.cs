namespace Blazelike.Game.Extensions;

public static class Geom
{
    public static bool Intersects((double X, double Y) a1,
                                  (double X, double Y) a2,
                                  (double X, double Y) b1,
                                  (double X, double Y) b2,
                                  out (double X, double Y) intersection)
    {
        intersection = (0, 0);

        (double X, double Y) b = (a2.X - a1.X, a2.Y - a1.Y);
        (double X, double Y) d = (b2.X - b1.X, b2.Y - b1.Y);
        var bDotDPerp = b.X * d.Y - b.Y * d.X;

        // if b dot d == 0, it means the lines are parallel so have infinite intersection points
        if (bDotDPerp == 0)
        {
            return false;
        }

        (double X, double Y) c = (b1.X - a1.X, b1.Y - a1.Y);
        var t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
        if (t < 0 || t > 1)
        {
            return false;
        }

        var u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
        if (u < 0 || u > 1)
        {
            return false;
        }

        intersection = (a1.X + t * b.X, a1.Y + t * b.Y);

        return true;
    }
}