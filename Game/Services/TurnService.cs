using Blazelike.Game.Properties;

namespace Blazelike.Game.Services;

public class TurnService
{
    private readonly PropertyService _propertyService;
    private readonly int MaxAgility = 20;

    public TurnService(PropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    public int CurrentTurn { get; private set; }
    public IEnumerable<(int Order, Entity Entity)> Order => SortedQueue.AsReadOnly();
    private List<(int Order, Entity Entity)> SortedQueue { get; set; } = new();

    public void AdvanceTurn()
    {
        if (TryGetCurrentEntity(out var turn, out var entity))
        {
            SortedQueue.RemoveAll(e => e.Order == turn && e.Entity == entity);
            Register(entity);
        }
        if (!SortedQueue.Any(x => x.Order == CurrentTurn))
        {
            CurrentTurn++;
        }
    }

    public bool CanMakeTurn(Entity entity)
    {
        return SortedQueue.First().Entity == entity;
    }

    public void Register(Entity entity)
    {
        if (_propertyService.TryGet<int>(entity.Id, PropertyTypes.Agility, out var agility))
        {
            Insert(entity, agility.Value);
        }
        else
        {
            throw new InvalidOperationException($"Entity has no Agility");
        }
    }

    public bool TryGetCurrentEntity(out Entity entity)
    {
        return TryGetCurrentEntity(out _, out entity);
    }

    private void Insert(Entity entity, int agility)
    {
        SortedQueue.Add((CurrentTurn + (MaxAgility - agility), entity));
        SortedQueue = SortedQueue.OrderBy(x => x.Order).ToList();
    }

    private bool TryGetCurrentEntity(out int turn, out Entity entity)
    {
        (turn, entity) = SortedQueue.FirstOrDefault(x => x.Order == CurrentTurn, (-1, null));
        if (turn == -1 || entity is null)
        {
            entity = null;
            return false;
        }
        return true;
    }
}