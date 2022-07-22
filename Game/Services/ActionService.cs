using Blazelike.Game.Actions;

namespace Blazelike.Game.Services;

public class ActionService
{
    private readonly Dictionary<Guid, List<IAction>> _entityActions = new();
    private readonly Dictionary<Guid, List<IAction>> _passiveActions = new();

    public void ActPassive(Guid id)
    {
        if (!_passiveActions.TryGetValue(id, out var actions))
        {
            return;
        }
        foreach (var action in actions.OfType<IAction>())
        {
            if (action.TryPrepare(0, 0))
            {
                action.Act();
            }
        }
    }

    public void Add(Guid id, IAction action)
    {
        if (!_entityActions.TryGetValue(id, out var actions))
        {
            actions = new List<IAction>();
            _entityActions[id] = actions;
        }
        actions.Add(action);
    }

    public void AddPassive(Guid id, IAction action)
    {
        if (!_passiveActions.TryGetValue(id, out var actions))
        {
            actions = new List<IAction>();
            _passiveActions[id] = actions;
        }
        actions.Add(action);
    }

    public bool TryAct(Guid id, int x, int y)
    {
        if (!_entityActions.TryGetValue(id, out var actions))
        {
            ActPassive(id);
            return false;
        }
        foreach (var action in actions.OfType<IAction>())
        {
            if (action.TryPrepare(x, y))
            {
                action.Act();
                ActPassive(id);
                return true;
            }
        }

        ActPassive(id);
        return false;
    }

    internal void Remove(Guid id)
    {
        _entityActions.Remove(id);
    }
}