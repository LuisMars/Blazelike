using Blazelike.Game.Maps;
using Blazelike.Game.Properties;
using Blazelike.Game.Services;

namespace Blazelike.Game.Actions;

public class DieAction : IAction
{
    protected readonly LoggerService _loggerService;
    private readonly PropertyService _propertyService;

    public DieAction(PropertyService propertyService, LoggerService loggerService, Entity entity, World world)
    {
        _loggerService = loggerService;
        _propertyService = propertyService;
        Entity = entity;
        World = world;
    }

    public Entity Entity { get; }
    public World World { get; }

    public bool Act()
    {
        World.RemoveEntity(Entity);
        _loggerService.Log($"{Entity.Name} died.");
        return true;
    }

    public bool TryPrepare(int x, int y)
    {
        if (!_propertyService.TryGet<int>(Entity.Id, PropertyTypes.Health, out var property))
        {
            return false;
        }
        return property.Value <= property.MinValue;
    }
}