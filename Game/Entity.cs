namespace Blazelike.Game;

public class Entity
{
    public Entity(string name, int x, int y, string icon, string character, bool walkable)
    {
        Name = name;
        Icon = icon;
        Character = character;
        Walkable = walkable;
        Position = (x, y);
    }

    public int BackgroundColor { get; private set; } = 2;
    public string Character { get; private set; }
    public int Color { get; private set; } = 4;
    public string Icon { get; private set; }
    public string Name { get; }
    public (int X, int Y) Position { get; internal set; }
    public bool Walkable { get; private set; }

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