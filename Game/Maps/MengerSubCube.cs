using Blazelike.Game.Extensions;

namespace Blazelike.Game.Maps;

public class MengerSubCube
{
    private readonly Dictionary<Direction, Map> _maps = new();
    private readonly List<MengerSubCube> _neighbours = new();

    public MengerSubCube((int X, int Y, int Z) position)
    {
        Position = position;

        var topBackLeft = (X, Y, Z);
        var topBackRight = (X + 1, Y, Z);
        var topFrontLeft = (X, Y, Z + 1);
        var topFrontRight = (X + 1, Y, Z + 1);

        var bottomBackLeft = (X, Y + 1, Z);
        var bottomBackRight = (X + 1, Y + 1, Z);
        var bottomFrontLeft = (X, Y + 1, Z + 1);
        var bottomFrontRight = (X + 1, Y + 1, Z + 1);

        _maps[Direction.Up] = new(topBackLeft, topBackRight, topFrontRight, topFrontRight);
        _maps[Direction.Down] = new(bottomBackLeft, bottomBackRight, bottomFrontRight, bottomFrontRight);
        _maps[Direction.Left] = new(topBackLeft, topFrontLeft, bottomFrontLeft, bottomBackLeft);
        _maps[Direction.Right] = new(topBackRight, topFrontRight, bottomFrontRight, bottomBackRight);
        _maps[Direction.Back] = new(topBackLeft, topBackRight, bottomBackRight, bottomBackLeft);
        _maps[Direction.Forward] = new(topFrontLeft, topFrontRight, bottomFrontRight, bottomFrontLeft);
    }

    public bool HasMaps => IsCenter;
    public (int X, int Y, int Z) Position { get; }
    public int X => Position.X;
    public int Y => Position.Y;
    public int Z => Position.Z;
    private bool IsCenter => IsCenterX || IsCenterY || IsCenterZ;
    private bool IsCenterX => Y == 1 && Z == 1;

    private bool IsCenterY => X == 1 && Z == 1;

    private bool IsCenterZ => X == 1 && Y == 1;

    public Map this[Direction direction] => _maps[direction];

    public void CleanHiddenMaps(MengerSubCube other)
    {
        if (!IsNextTo(other))
        {
            return;
        }
        _neighbours.Add(other);
        if (X == other.X)
        {
            if (Y == other.Y)
            {
                if (Z == other.Z + 1)
                {
                    Remove(Direction.Forward);
                }
                if (Z == other.Z - 1)
                {
                    Remove(Direction.Back);
                }
            }
            if (Z == other.Z)
            {
                if (Y == other.Y + 1)
                {
                    Remove(Direction.Down);
                }
                if (Y == other.Y - 1)
                {
                    Remove(Direction.Up);
                }
            }
        }
        if (Z == other.Z)
        {
            if (Y == other.Y)
            {
                if (X == other.X + 1)
                {
                    Remove(Direction.Right);
                }
                if (X == other.X - 1)
                {
                    Remove(Direction.Left);
                }
            }
        }
    }

    public bool IsNextTo(MengerSubCube other)
    {
        return Position.DistanceSquared(other.Position) <= 1;
    }

    protected void Remove(Direction direction)
    {
        if (!_maps.ContainsKey(direction))
        {
            return;
        }
        _maps.Remove(direction);
    }
}