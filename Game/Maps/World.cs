namespace Blazelike.Game.Maps;

/// <summary>
///
///     ______x
///    /|
///   /z|
///  /  |
/// /   | y
///
/// </summary>
public class World
{
    private readonly Dictionary<(int X, int Y, int Z), MengerSubCube> _cubes = new();

    public World()
    {
        InitializeCubes();
    }

    private void InitializeCubes()
    {
        for (var x = 0; x < 3; x++)
        {
            for (var y = 0; y < 3; y++)
            {
                for (var z = 0; z < 3; z++)
                {
                    if ((x == 1 && y == 1) ||
                        (x == 1 && z == 1) ||
                        (y == 1 && z == 1))
                    {
                        continue;
                    }
                    _cubes[(x, y, z)] = new MengerSubCube((x, y, z));
                }
            }
        }

        foreach (var cube in _cubes.Values)
        {
            foreach (var other in _cubes.Values)
            {
                if (cube == other)
                {
                    continue;
                }
                cube.CleanHiddenMaps(other);
            }
        }
    }
}