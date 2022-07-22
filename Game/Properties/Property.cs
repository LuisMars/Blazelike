namespace Blazelike.Game.Properties;

public class Property<T> : PropertyBase
{
    public Property(T value)
    {
        Value = value;
        MaxValue = value;
    }

    public T MaxValue { get; set; }
    public T MinValue { get; set; }
    public T Value { get; set; }
}