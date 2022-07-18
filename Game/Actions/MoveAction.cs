using Blazelike.Game.Maps;
using Blazelike.Game.Services;

namespace Blazelike.Game.Actions;

public class MoveAction : DirectionalAction
{
    public MoveAction(LoggerService loggerService, Entity entity, World world) : base(loggerService, entity, world)
    {
        FailIfFoundSomething = true;
    }

    protected override bool FindCondition(Entity e)
    {
        return !e.Walkable;
    }

    protected override void LogOnNoFreeSpace(Entity entity)
    {
        _loggerService.AddLog($"{Entity.Name} can't move to {CurrentMap.Position} ({NextX}, {NextY}), because there is a {entity.Name}");
    }

    protected override void LogOnSuccess()
    {
        if (Entity.IsPlayer)
        {
            _loggerService.AddLog($"{Entity.Name} moved to {CurrentMap.Position} ({NextX}, {NextY})");
        }
    }

    protected override void OnAct()
    {
        MoveBetweenMaps();
        if (Entity.IsPlayer && World.CurrentMap == CurrentMap)
        {
            CurrentMap.Visited = true;
        }
        Entity.SetPosition(NextX, NextY);
    }

    private void MoveBetweenMaps()
    {
        if (NextX == 0)
        {
            var nextMapPos = (CurrentMap.Position.X - 1, CurrentMap.Position.Y);
            MoveMaps(nextMapPos);
            NextX = CurrentMap.Width - 1;
        }
        else if (NextY == 0)
        {
            var nextMapPos = (CurrentMap.Position.X, CurrentMap.Position.Y - 1);
            MoveMaps(nextMapPos);
            NextY = CurrentMap.Height - 1;
        }
        else if (NextX == CurrentMap.Width - 1)
        {
            var nextMapPos = (CurrentMap.Position.X + 1, CurrentMap.Position.Y);
            MoveMaps(nextMapPos);
            NextX = 0;
        }
        else if (NextY == CurrentMap.Height - 1)
        {
            var nextMapPos = (CurrentMap.Position.X, CurrentMap.Position.Y + 1);
            MoveMaps(nextMapPos);
            NextY = 0;
        }
    }

    private void MoveMaps((int x, int y) nextMapPos)
    {
        CurrentMap.Entities.Remove(Entity);
        if (Entity.IsPlayer)
        {
            World.SetCurrentMap(nextMapPos);
        }
        Entity.Map = World.MapAt(nextMapPos);
        CurrentMap.Entities.Add(Entity);
    }
}