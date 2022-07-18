using Blazelike.Game.Services;

namespace Blazelike.Game.Maps;

public class Map
{
    private readonly LoggerService _loggerService;
    private readonly Random _random = new();

    public Map(EntitySpawner entitySpawner, LoggerService loggerService, (int X, int Y) position)
    {
        Board = new Entity[Width, Height];
        for (var i = 0; i < Width; i++)
        {
            Entities.Add(entitySpawner.CreateWall(this, i, 0));
            Entities.Add(entitySpawner.CreateWall(this, i, Height - 1));
            Entities.Add(entitySpawner.CreateWall(this, 0, i));
            Entities.Add(entitySpawner.CreateWall(this, Width - 1, i));
        }

        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                if (!Entities.Any(e => e.Position == (i, j)))
                {
                    Entities.Add(entitySpawner.CreateFloor(this, i, j));
                }
            }
        }

        Entities.Add(entitySpawner.CreateEnemy(this, Width / 2 + 1, Height / 2 + 1));

        for (var i = 0; i < 10; i++)
        {
            var x = 0;
            var y = 0;
            do
            {
                x = _random.Next(2, Width - 2);
                y = _random.Next(2, Width - 2);
            }
            while (Entities.Any(e => e.Position == (x, y) && !e.Walkable));

            Entities.Add(entitySpawner.CreateWall(this, x, y));
        }

        _loggerService = loggerService;
        Position = position;
    }

    public Entity?[,] Board { get; set; }
    public List<Entity> Entities { get; } = new();
    public int Height { get; set; } = 11;
    public (int X, int Y) Position { get; }
    public bool Visited { get; set; }
    public int Width { get; set; } = 11;

    internal void AddDoor(Map other)
    {
        var doorX = Width / 2;
        var doorY = Height / 2;
        var leftDoor = (0, doorY);
        var rightDoor = (Width - 1, doorY);
        var topDoor = (doorX, 0);
        var bottomDoor = (doorX, Height - 1);
        var isVertical = Position.X == other.Position.X;
        var isHorizontal = Position.Y == other.Position.Y;
        var otherIsLeft = Position.X > other.Position.X;
        var otherIsTop = Position.Y > other.Position.Y;
        if (isVertical)
        {
            if (otherIsTop)
            {
                Entities.First(x => x.Position == topDoor).SetAsDoor();
                other.Entities.First(x => x.Position == bottomDoor).SetAsDoor();
            }
            else
            {
                Entities.First(x => x.Position == bottomDoor).SetAsDoor();
                other.Entities.First(x => x.Position == topDoor).SetAsDoor();
            }
        }
        else if (isHorizontal)
        {
            if (otherIsLeft)
            {
                Entities.First(x => x.Position == leftDoor).SetAsDoor();
                other.Entities.First(x => x.Position == rightDoor).SetAsDoor();
            }
            else
            {
                Entities.First(x => x.Position == rightDoor).SetAsDoor();
                other.Entities.First(x => x.Position == leftDoor).SetAsDoor();
            }
        }
        else
        {
            _loggerService.LogConsole($"Can't place door at {Position} - {other.Position}");
        }
    }
}