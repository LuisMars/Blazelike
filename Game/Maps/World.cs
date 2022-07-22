using Blazelike.Game.Extensions;
using Blazelike.Game.Properties;
using Blazelike.Game.Services;

namespace Blazelike.Game.Maps;

public class World
{
    private readonly ActionService _actionService;
    private readonly EntitySpawner _entitySpawner;
    private readonly LoggerService _loggerService;
    private readonly PropertyService _propertyService;
    private readonly Random _random = new();
    private readonly TurnService _turnService;

    public World()
    {
        _loggerService = new LoggerService();
        _actionService = new ActionService();
        _propertyService = new PropertyService();
        _turnService = new TurnService(_propertyService);
        _entitySpawner = new EntitySpawner(this, _loggerService, _actionService, _propertyService, _turnService);
        InitializeMaps();
        CurrentMap = Maps.Values.First();
        (var x, var y) = CurrentMap.FindEmptySpot();
        Player = _entitySpawner.CreatePlayer(CurrentMap, x, y);

        CurrentMap.Entities.Add(Player);
        CurrentMap.Visited = true;
    }

    public Property<int> CurrentHealth
    {
        get
        {
            if (!_propertyService.TryGet<int>(Player.Id, PropertyTypes.Health, out var health))
            {
                return null;
            }
            return health;
        }
    }

    public Map CurrentMap { get; set; }

    public int CurrentTurn => _turnService.CurrentTurn;

    public int DelayMs { get; set; } = 3000 / 60;
    public IEnumerable<string> EntityOrder => _turnService.Order.Where(x => x.Entity.Map == CurrentMap).Select(x => $"{x.Order}: {x.Entity.Name}");

    public bool GameOver
    {
        get
        {
            return !_propertyService.Has(Player.Id, PropertyTypes.Health);
        }
    }

    public int Height { get; set; } = 10;
    public bool IsRunning { get; private set; }
    public IReadOnlyList<string> Log => _loggerService.LogList;

    public Dictionary<(int X, int Y), Map> Maps { get; } = new();

    public Entity Player { get; set; }

    public Action Refresh { get; internal set; }

    public int Width { get; set; } = 10;

    public (int X, int Y) GetDirectionFromPlayer(int x, int y)
    {
        var baseX = x - Player.Position.X;
        var baseY = y - Player.Position.Y;
        var amountX = Math.Sign(baseX);
        var amountY = Math.Sign(baseY);
        if (Math.Abs(baseX) > Math.Abs(baseY))
        {
            amountY = 0;
        }
        else
        {
            amountX = 0;
        }
        return (amountX, amountY);
    }

    public (int X, int Y) GetDirectionToPlayer((int X, int Y) position)
    {
        var (X, Y) = GetDirectionFromPlayer(position.X, position.Y);
        return (-X, -Y);
    }

    public Map MapAt((int x, int y) nextMapPos)
    {
        return Maps[nextMapPos];
    }

    public async Task MoveByAsync(int x, int y)
    {
        if (IsRunning)
        {
            return;
        }
        IsRunning = true;
        Refresh();
        _actionService.TryAct(Player.Id, x, y);
        _turnService.AdvanceTurn();
        Update();
        await Task.Delay(DelayMs);
        await ActUntilIsPlayersTurnAsync();
        Update();
        IsRunning = false;
        Refresh();
    }

    public void SetCurrentMap((int x, int y) nextMapPos)
    {
        CurrentMap = Maps[nextMapPos];
    }

    public void Update(bool updateVisibility = true)
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
            if (!entity.IsVisible && entity.WasVisible && !entity.VisibleInShadows)
            {
                continue;
            }
            var (x, y) = entity.Position;
            CurrentMap.Board[x, y] = entity;
        }
        if (updateVisibility)
        {
            UpdateVisibility();
        }
        Refresh();
    }

    internal void RemoveEntity(Entity entity)
    {
        _actionService.Remove(entity.Id);
        _propertyService.Remove(entity.Id);
        _turnService.Remove(entity.Id);
        Maps.Values.ToList().ForEach(m => m.Remove(entity.Id));
    }

    private async Task ActUntilIsPlayersTurnAsync()
    {
        _turnService.TryGetCurrentEntity(out var nextEntity);
        while (nextEntity != Player)
        {
            if (nextEntity is not null && nextEntity.Map.Position.DistanceSquared(Player.Map.Position) <= 1)
            {
                var dir = GetRandomDirection();
                if (_random.NextDouble() < 0.75)
                {
                    dir = GetDirectionToPlayer(nextEntity.Position);
                }
                _actionService.TryAct(nextEntity.Id, dir.X, dir.Y);
                if (nextEntity.Map == Player.Map && nextEntity.IsVisible)
                {
                    Update(false);
                    await Task.Delay(DelayMs);
                }
            }
            _turnService.AdvanceTurn();
            _turnService.TryGetCurrentEntity(out nextEntity);
        }
    }

    private (int X, int Y) GetRandomDirection()
    {
        var dir = _random.Next(0, 4);
        return dir switch
        {
            0 => (-1, 0),
            1 => (1, 0),
            2 => (0, 1),
            3 => (0, -1),
            _ => throw new NotImplementedException(),
        };
    }

    private void InitializeMaps()
    {
        var x = 0;
        var y = 0;
        Map? lastMap = null;
        while (true)
        {
            var position = (x, y);
            if (!Maps.TryGetValue(position, out var currentMap))
            {
                currentMap = new Map(_entitySpawner, _loggerService, position);
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
                    x++;
                }
                else if (y < Height - 1)
                {
                    y++;
                }
            }
            else
            {
                if (_random.NextDouble() > 0.5 && x > 0)
                {
                    x--;
                }
                else if (y > 0)
                {
                    y--;
                }
            }
        }
    }

    private void UpdateVisibility()
    {
        foreach (var entity in CurrentMap.Entities)
        {
            if (entity == Player)
            {
                Player.IsVisible = true;
                Player.WasVisible = true;
                continue;
            }
            var visible = true;

            var start = (Player.Position.X + 0.5, Player.Position.Y + 0.5);
            var end = (entity.Position.X + 0.5, entity.Position.Y + 0.5);

            foreach (var other in CurrentMap.Entities.Where(x => x != Player && x != entity && !x.Translucent && !(x.Position == Player.Position)))
            {
                var left = Geom.Intersects(start, end,
                    (other.Position.X + 0.0, other.Position.Y + 0.5),
                    (other.Position.X + 0.5, other.Position.Y + 0.0), out _);
                if (left)
                {
                    visible = false;
                    break;
                }
                var right = Geom.Intersects(start, end,
                    (other.Position.X + 0.5, other.Position.Y + 0.0),
                    (other.Position.X + 1.0, other.Position.Y + 0.5), out _);
                if (right)
                {
                    visible = false;
                    break;
                }
                var top = Geom.Intersects(start, end,
                    (other.Position.X + 1.0, other.Position.Y + 0.5),
                    (other.Position.X + 0.5, other.Position.Y + 1.0), out _);
                if (top)
                {
                    visible = false;
                    break;
                }
                var bottom = Geom.Intersects(start, end,
                    (other.Position.X + 0.5, other.Position.Y + 1.0),
                    (other.Position.X + 0.0, other.Position.Y + 0.5), out _);
                if (bottom)
                {
                    visible = false;
                    break;
                }
            }

            entity.IsVisible = visible;
            if (visible)
            {
                entity.WasVisible = true;
            }
        }
    }
}