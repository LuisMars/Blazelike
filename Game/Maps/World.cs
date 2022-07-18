using Blazelike.Game.Extensions;
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
        Player = _entitySpawner.CreatePlayer(CurrentMap, CurrentMap.Width / 2, CurrentMap.Height / 2);

        CurrentMap.Entities.Add(Player);
        CurrentMap.Visited = true;
    }

    public Map CurrentMap { get; set; }
    public int CurrentTurn => _turnService.CurrentTurn;
    public int DelayMs { get; set; } = 100;
    public IEnumerable<string> EntityOrder => _turnService.Order.Where(x => x.Entity.Map == CurrentMap).Select(x => $"{x.Order}: {x.Entity.Name}");
    public int Height { get; set; } = 10;
    public IReadOnlyList<string> Log => _loggerService.Log;
    public Dictionary<(int X, int Y), Map> Maps { get; } = new();
    public Entity Player { get; set; }
    public Action Refresh { get; internal set; }
    public int Width { get; set; } = 10;

    public Map MapAt((int x, int y) nextMapPos)
    {
        return Maps[nextMapPos];
    }

    public async Task MoveByAsync(int x, int y)
    {
        //await ActUntilIsPlayersTurnAsync();

        _actionService.TryAct(Player.Id, x, y);
        _turnService.AdvanceTurn();

        Update();
        await Task.Delay(DelayMs);

        await ActUntilIsPlayersTurnAsync();
    }

    public void SetCurrentMap((int x, int y) nextMapPos)
    {
        CurrentMap = Maps[nextMapPos];
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
        UpdateVisibility();
        Refresh();
    }

    private async Task ActUntilIsPlayersTurnAsync()
    {
        _turnService.TryGetCurrentEntity(out var nextEntity);
        while (nextEntity != Player)
        {
            if (nextEntity is not null)
            {
                var dir = _random.Next(0, 4);
                (var x, var y) = dir switch
                {
                    0 => (-1, 0),
                    1 => (1, 0),
                    2 => (0, 1),
                    3 => (0, -1),
                    _ => throw new NotImplementedException(),
                };
                _actionService.TryAct(nextEntity.Id, x, y);
                if (nextEntity.Map == Player.Map)
                {
                    Update();
                    await Task.Delay(DelayMs);
                }
            }
            _turnService.AdvanceTurn();
            _turnService.TryGetCurrentEntity(out nextEntity);
        }

        Update();
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
                Player.Visible = true;
                Player.WasVisible = true;
                continue;
            }
            var visible = true;

            var start = (Player.Position.X + 0.5, Player.Position.Y + 0.5);
            var end = (entity.Position.X + 0.5, entity.Position.Y + 0.5);

            foreach (var other in CurrentMap.Entities)
            {
                if (other == Player || other == entity || other.Translucent || other.Position == Player.Position)
                {
                    continue;
                }

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

            entity.Visible = visible;
            if (visible)
            {
                entity.WasVisible = true;
            }
        }
    }
}