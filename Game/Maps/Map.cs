namespace Blazelike.Game.Maps;

public class Map
{
    private readonly Action<string> _log;

    public Map(Action<string> log, (int X, int Y) position)
    {
        Board = new Entity[Width, Height];
        for (var i = 0; i < Width; i++)
        {
            Entities.Add(new Entity("Wall", i, 0, "wall", "#", false));
            Entities.Add(new Entity("Wall", i, Height - 1, "wall", "#", false));
            Entities.Add(new Entity("Wall", 0, i, "wall", "#", false));
            Entities.Add(new Entity("Wall", Width - 1, i, "wall", "#", false));
        }

        _log = log;
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
            _log($"Can't place door at {Position} - {other.Position}");
        }
    }
}