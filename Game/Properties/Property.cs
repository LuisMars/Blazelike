namespace Blazelike.Game.Properties;

public class Property<T> : PropertyBase
{
    public T Value { get; set; }

    public Property(T value)
    {
        Value = value;
    }
}