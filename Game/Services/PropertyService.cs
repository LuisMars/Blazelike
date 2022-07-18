using Blazelike.Game.Properties;

namespace Blazelike.Game.Services;

public class PropertyService
{
    private readonly Dictionary<Guid, Dictionary<PropertyTypes, PropertyBase>> _properties = new();

    public void Set(Guid id, PropertyTypes type, PropertyBase property)
    {
        Console.WriteLine($"id: {id}, name: {type}, property; {property}");
        if (!_properties.ContainsKey(id))
        {
            var propertyDict = new Dictionary<PropertyTypes, PropertyBase>();
            _properties[id] = propertyDict;
        }
        _properties[id][type] = property;
    }

    public bool TryGet<T>(Guid id, PropertyTypes type, out Property<T> property)
    {
        property = null;
        if (!_properties.TryGetValue(id, out var propertyDict))
        {
            return false;
        }
        if (!propertyDict.TryGetValue(type, out var extractedProperty))
        {
            return false;
        }
        property = extractedProperty as Property<T>;
        return true;
    }

    internal bool Has(Guid id, PropertyTypes type)
    {
        if (!_properties.TryGetValue(id, out var propertyDict))
        {
            return false;
        }
        return propertyDict.ContainsKey(type);
    }
}