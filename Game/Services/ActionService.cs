using Blazelike.Game.Actions;

namespace Blazelike.Game.Services;

public class ActionService
{
    private readonly Dictionary<Guid, List<IAction>> _entityActions = new();

    public void Add(Guid id, IAction action)
    {
        if (!_entityActions.TryGetValue(id, out var actions))
        {
            actions = new List<IAction>();
            _entityActions[id] = actions;
        }
        actions.Add(action);
    }

    public bool TryAct(Guid id, int x, int y)
    {
        if (!_entityActions.TryGetValue(id, out var actions))
        {
            return false;
        }
        foreach (var action in actions.OfType<IAction>())
        {
            if (action.TryPrepare(x, y))
            {
                action.Act();
                return true;
            }
        }
        return false;
    }
}