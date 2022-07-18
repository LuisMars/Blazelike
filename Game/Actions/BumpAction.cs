using Blazelike.Game.Maps;
using Blazelike.Game.Properties;
using Blazelike.Game.Services;

namespace Blazelike.Game.Actions;

public class BumpAction : DirectionalAction
{
    private readonly PropertyService _propertyService;

    public BumpAction(PropertyService propertyService,
                      LoggerService loggerService,
                      Entity entity,
                      World world) : base(loggerService, entity, world)
    {
        _propertyService = propertyService;
    }

    protected override bool FindCondition(Entity e)
    {
        return e.IsPlayer != Entity.IsPlayer &&
               _propertyService.Has(e.Id, PropertyTypes.Health);
    }

    protected override void LogOnNoFreeSpace(Entity foundSomething)
    {
    }

    protected override void LogOnSuccess()
    {
        _loggerService.AddLog($"{Entity.Name} attacked {Other.Name}");
    }

    protected override void OnAct()
    {
    }
}