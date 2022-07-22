namespace Blazelike.Game.Entities;

public class EntityDescriptor
{
    public int Agility { get; set; }
    public string Character { get; set; }
    public int Color { get; set; }
    public int Health { get; set; }
    public string Icon { get; set; }
    public string Name { get; set; }
    public int Attack { get; internal set; }
}