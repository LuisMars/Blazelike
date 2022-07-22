using Blazelike.Game.Maps;
using Blazelike.Game.Services;

namespace Blazelike.Game.Actions;

public abstract class DirectionalAction : IAction
{
    protected readonly LoggerService _loggerService;

    public DirectionalAction(LoggerService loggerService, Entity entity, World world)
    {
        _loggerService = loggerService;
        Entity = entity;
        World = world;
    }

    public Map CurrentMap => Entity.Map;
    public Entity Entity { get; }
    public Entity? Other { get; private set; }
    public World World { get; }
    protected bool FailIfFoundSomething { get; set; }
    protected bool HadFreeSpace { get; set; }
    protected int NextX { get; set; }
    protected int NextY { get; set; }

    public bool Act()
    {
        if (!HadFreeSpace)
        {
            return false;
        }
        OnAct();
        LogOnSuccess();
        AfterAct();
        return true;
    }

    public bool TryPrepare(int x, int y)
    {
        NextX = Entity.Position.X + x;
        NextY = Entity.Position.Y + y;
        HadFreeSpace = HasFreeSpace(false);
        return HadFreeSpace;
    }

    protected virtual void AfterAct()
    {
    }

    protected abstract bool FindCondition(Entity e);

    protected bool HasFreeSpace(bool log)
    {
        if (IsInBounds())
        {
            return false;
        }

        Other = CurrentMap.Entities.FirstOrDefault(e => SatisfiesConditions(e));
        if (Other is not null)
        {
            if (Entity.IsPlayer && log)
            {
                LogOnNoFreeSpace(Other);
            }
            return !FailIfFoundSomething;
        }
        return FailIfFoundSomething;
    }

    protected bool IsInBounds()
    {
        return NextX < 0 || NextX >= CurrentMap.Width || NextY < 0 || NextY >= CurrentMap.Height;
    }

    protected virtual void LogOnNoFreeSpace(Entity foundSomething)
    {
    }

    protected abstract void LogOnSuccess();

    protected abstract void OnAct();

    private bool SatisfiesConditions(Entity e)
    {
        var samePosition = Entity.Map == e.Map && e.Position == (NextX, NextY);
        var findCondition = FindCondition(e);
        return samePosition && findCondition;
    }
}