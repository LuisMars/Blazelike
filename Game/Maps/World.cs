namespace Blazelike.Game.Maps;

public class World
{
    private readonly Random _random = new();

    public World()
    {
        InitializeMaps();
        CurrentMap = Maps.Values.First();
        Player = new Entity("You", CurrentMap.Width / 2, CurrentMap.Height / 2, "person", "@", false);
        CurrentMap.Entities.Add(Player);
        CurrentMap.Visited = true;
    }

    public Map CurrentMap { get; set; }
    public int Height { get; set; } = 10;
    public List<string> Log { get; } = new();
    public Dictionary<(int X, int Y), Map> Maps { get; } = new();
    public Entity Player { get; set; }
    public int Width { get; set; } = 10;

    public void AddLog(string log)
    {
        Console.WriteLine(log);
        Log.Insert(0, log);
        var count = Log.Count;
        if (count > 30)
        {
            Log.RemoveAt(count - 1);
        }
    }

    public void MoveBy(int x, int y)
    {
        var nextX = Player.Position.X + x;
        var nextY = Player.Position.Y + y;
        if (nextX < 0 || nextX >= CurrentMap.Width || nextY < 0 || nextY >= CurrentMap.Height)
        {
            return;
        }
        var foundSomething = CurrentMap.Entities.FirstOrDefault(e => e.Position == (nextX, nextY) && !e.Walkable);
        if (foundSomething is not null)
        {
            AddLog($"{Player.Name} can't move to {CurrentMap.Position} ({nextX}, {nextY}), because there is a {foundSomething.Name}");
            return;
        }
        if (nextX == 0)
        {
            CurrentMap.Entities.Remove(Player);
            CurrentMap = Maps[(CurrentMap.Position.X - 1, CurrentMap.Position.Y)];
            nextX = CurrentMap.Width - 1;
            CurrentMap.Entities.Add(Player);
        }
        else if (nextY == 0)
        {
            CurrentMap.Entities.Remove(Player);
            CurrentMap = Maps[(CurrentMap.Position.X, CurrentMap.Position.Y - 1)];
            nextY = CurrentMap.Height - 1;
            CurrentMap.Entities.Add(Player);
        }
        else if (nextX == CurrentMap.Width - 1)
        {
            CurrentMap.Entities.Remove(Player);
            CurrentMap = Maps[(CurrentMap.Position.X + 1, CurrentMap.Position.Y)];
            nextX = 0;
            CurrentMap.Entities.Add(Player);
        }
        else if (nextY == CurrentMap.Height - 1)
        {
            CurrentMap.Entities.Remove(Player);
            CurrentMap = Maps[(CurrentMap.Position.X, CurrentMap.Position.Y + 1)];
            nextY = 0;
            CurrentMap.Entities.Add(Player);
        }
        CurrentMap.Visited = true;
        Player.SetPosition(nextX, nextY);
        AddLog($"{Player.Name} moved to {CurrentMap.Position} ({nextX}, {nextY})");
        Update();
    }

    public void Update()
    {
        for (var x = 0; x < CurrentMap.Width; x++)
        {
            for (var y = 0; y < CurrentMap.Height; y++)
            {
                CurrentMap.Board[x, y] = null;
            }
        }
        foreach (var entity in CurrentMap.Entities)
        {
            var (x, y) = entity.Position;
            CurrentMap.Board[x, y] = entity;
        }
    }

    private void InitializeMaps()
    {
        var x = 0;
        var y = 0;
        var lastX = 0;
        var lastY = 0;
        Map? lastMap = null;
        while (true)
        {
            var position = (x, y);
            if (!Maps.TryGetValue(position, out var currentMap))
            {
                currentMap = new Map(AddLog, position);
                Maps[position] = currentMap;
            }
            if (lastMap is not null && lastMap.Position != currentMap.Position)
            {
                lastMap.AddDoor(currentMap);
                lastMap = currentMap;
            }
            if (lastMap is null)
            {
                lastMap = currentMap;
            }

            if (x == Width - 1 && y == Height - 1)
            {
                break;
            }
            if (_random.NextDouble() > 0.5)
            {
                if (_random.NextDouble() > 0.5 && x < Width - 1)
                {
                    lastX = x;
                    x++;
                }
                else if (y < Height - 1)
                {
                    lastY = y;
                    y++;
                }
            }
            else
            {
                if (_random.NextDouble() > 0.5 && x > 0)
                {
                    lastX = x;
                    x--;
                }
                else if (y > 0)
                {
                    lastY = y;
                    y--;
                }
            }
        }
    }
}