namespace Blazelike.Game.Maps;

public class Map
{
    public Map(params (int X, int Y, int Z)[] positions)
    {
        Board = new Entity[Width, Height];
        for (var i = 0; i < Width; i++)
        {
            if (i == Width / 2)
            {
                continue;
            }
            Entities.Add(new Entity("Wall", i, 0, "wall", "#", false));
            Entities.Add(new Entity("Wall", i, Height - 1, "wall", "#", false));
            Entities.Add(new Entity("Wall", 0, i, "wall", "#", false));
            Entities.Add(new Entity("Wall", Width - 1, i, "wall", "#", false));
        }
    }

    public Entity?[,] Board { get; set; }
    public int Height { get; set; } = 11;
    public int Width { get; set; } = 11;
    private List<Entity> Entities { get; set; } = new();
}