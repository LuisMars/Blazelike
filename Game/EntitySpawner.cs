using Blazelike.Game.Actions;
using Blazelike.Game.Entities;
using Blazelike.Game.Maps;
using Blazelike.Game.Properties;
using Blazelike.Game.Services;

namespace Blazelike.Game;

public class EntitySpawner
{
    private readonly ActionService _actionService;
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

    internal Entity CreateEnemy(Map map, int x, int y)
    {
        var enemyDescriptor = new EntityDescriptor
        {
            Color = 7,
            Name = "Skeleton",
            Icon = "skeleton",
            Character = "%",
            Agility = 5,
            Health = 10
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
            Agility = 10,
            Health = 10
        };
        var player = CreateCreature(map, x, y, playerDescriptor);
        player.IsPlayer = true;

        return player;
    }

    private Entity CreateCreature(Map map, int x, int y, EntityDescriptor descriptor)
    {
        var player = new Entity(map, descriptor.Name, x, y, descriptor.Icon, descriptor.Character, false, false)
        {
            Color = descriptor.Color,
            VisibleInShadows = false
        };
        _actionService.Add(player.Id, new MoveAction(_loggerService, player, World));
        _actionService.Add(player.Id, new BumpAction(_propertyService, _loggerService, player, World));
        _propertyService.Set(player.Id, PropertyTypes.Agility, new Property<int>(descriptor.Agility));
        _propertyService.Set(player.Id, PropertyTypes.Health, new Property<int>(descriptor.Health));
        _turnService.Register(player);
        return player;
    }
}