using Blazelike.Game.Actions;
using Blazelike.Game.Entities;
using Blazelike.Game.Maps;
using Blazelike.Game.Properties;
using Blazelike.Game.Services;

namespace Blazelike.Game;

public class EntitySpawner
{
    private readonly ActionService _actionService;

    private readonly Dictionary<EnemyType, int> _agility = new()
    {
        { EnemyType.Skeleton, 13 },
        { EnemyType.Goblin, 17 },
        { EnemyType.Troll, 7 },
    };

    private readonly Dictionary<EnemyType, int> _attack = new()
    {
        { EnemyType.Skeleton, 2 },
        { EnemyType.Goblin, 1 },
        { EnemyType.Troll, 6 },
    };

    private readonly Dictionary<EnemyType, string> _character = new()
    {
        { EnemyType.Skeleton, "%" },
        { EnemyType.Goblin, "g" },
        { EnemyType.Troll, "T" },
    };

    private readonly Dictionary<EnemyType, int> _color = new()
    {
        { EnemyType.Skeleton, 7 },
        { EnemyType.Goblin, 10 },
        { EnemyType.Troll, 10 },
    };

    private readonly Dictionary<EnemyType, int> _health = new()
    {
        { EnemyType.Skeleton, 3 },
        { EnemyType.Goblin, 1 },
        { EnemyType.Troll, 6 },
    };

    private readonly Dictionary<EnemyType, string> _icon = new()
    {
        { EnemyType.Skeleton, "skeleton" },
        { EnemyType.Goblin, "goblin" },
        { EnemyType.Troll, "troll" },
    };

    private readonly LoggerService _loggerService;
    private readonly PropertyService _propertyService;
    private readonly TurnService _turnService;

    public EntitySpawner(World world,
                         LoggerService loggerService,
                         ActionService actionService,
                         PropertyService propertyService,
                         TurnService turnService)
    {
        World = world;
        _loggerService = loggerService;
        _actionService = actionService;
        _propertyService = propertyService;
        _turnService = turnService;
    }

    public World World { get; }

    public Entity CreateFloor(Map map, int x, int y)
    {
        return new Entity(map, "Floor", x, y, "", " ", true, true)
        {
            Color = 3,
            BackgroundColor = 4
        };
    }

    public Entity CreateWall(Map map, int x, int y)
    {
        return new Entity(map, "Wall", x, y, "wall", "#", false, false)
        {
            Color = 6
        };
    }

    internal Entity CreateEnemy(EnemyType type, Map map, int x, int y)
    {
        var enemyDescriptor = new EntityDescriptor
        {
            Color = _color[type],
            Name = type.ToString(),
            Icon = _icon[type],
            Character = _character[type],
            Attack = _attack[type],
            Agility = _agility[type],
            Health = _health[type]
        };
        var enemy = CreateCreature(map, x, y, enemyDescriptor);
        return enemy;
    }

    internal Entity CreatePlayer(Map map, int x, int y)
    {
        var playerDescriptor = new EntityDescriptor
        {
            Color = 7,
            Name = "You",
            Icon = "person",
            Character = "@",
            Attack = 2,
            Agility = 10,
            Health = 20
        };
        var player = CreateCreature(map, x, y, playerDescriptor);
        player.IsPlayer = true;

        return player;
    }

    private Entity CreateCreature(Map map, int x, int y, EntityDescriptor descriptor)
    {
        var entity = new Entity(map, descriptor.Name, x, y, descriptor.Icon, descriptor.Character, false, false)
        {
            Color = descriptor.Color,
            VisibleInShadows = false
        };
        _actionService.Add(entity.Id, new MoveAction(_loggerService, entity, World));
        _actionService.Add(entity.Id, new BumpAction(_actionService, _propertyService, _loggerService, entity, World));
        _actionService.AddPassive(entity.Id, new DieAction(_propertyService, _loggerService, entity, World));
        _propertyService.Set(entity.Id, PropertyTypes.Agility, new Property<int>(descriptor.Agility));
        _propertyService.Set(entity.Id, PropertyTypes.Health, new Property<int>(descriptor.Health));
        _propertyService.Set(entity.Id, PropertyTypes.Attack, new Property<int>(descriptor.Attack));
        _turnService.Register(entity);
        return entity;
    }
}