using Blazelike.Game.Maps;
using Blazelike.Game.Properties;
using Blazelike.Game.Services;

namespace Blazelike.Game.Actions;

public class BumpAction : DirectionalAction
{
    private readonly ActionService _actionService;
    private readonly PropertyService _propertyService;

    public BumpAction(ActionService _actionService,
                      PropertyService propertyService,
                      LoggerService loggerService,
                      Entity entity,
                      World world) : base(loggerService, entity, world)
    {
        this._actionService = _actionService;
        _propertyService = propertyService;
    }

    protected override void AfterAct()
    {
        _actionService.ActPassive(Other.Id);
    }

    protected override bool FindCondition(Entity e)
    {
        return e.IsPlayer != Entity.IsPlayer &&
               _propertyService.Has(e.Id, PropertyTypes.Health);
    }

    protected override void LogOnSuccess()
    {
        _loggerService.Log($"{Entity.Name} attacked {Other.Name}");
    }

    protected override void OnAct()
    {
        if (!_propertyService.TryGet<int>(Other.Id, PropertyTypes.Health, out var health))
        {
            return;
        }
        if (!_propertyService.TryGet<int>(Entity.Id, PropertyTypes.Attack, out var attack))
        {
            return;
        }
        health.Value -= attack.Value;
    }
}