using Blazelike.Game.Maps;

namespace Blazelike.Game;

public class Entity
{
    public Entity(Map map, string name, int x, int y, string icon, string character, bool walkable, bool translucent)
    {
        Map = map;
        Name = name;
        Icon = icon;
        Character = character;
        Walkable = walkable;
        Translucent = translucent;
        Position = (x, y);
    }

    public int BackgroundColor { get; set; } = 4;
    public string Character { get; private set; }
    public int Color { get; set; } = 7;
    public string Icon { get; private set; }
    public Guid Id { get; } = Guid.NewGuid();
    public bool IsPlayer { get; internal set; }
    public Map Map { get; internal set; }
    public string Name { get; }
    public (int X, int Y) Position { get; internal set; }
    public bool Translucent { get; private set; }
    public bool IsVisible { get; set; }
    public bool VisibleInShadows { get; internal set; } = true;
    public bool Walkable { get; private set; }
    public bool WasVisible { get; set; }

    internal void SetAsDoor()
    {
        Walkable = true;
        Icon = "door";
        Character = "+";
    }

    internal void SetPosition(int x, int y)
    {
        Position = (x, y);
    }
}