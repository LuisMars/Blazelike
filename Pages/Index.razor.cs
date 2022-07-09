using Blazelike.Game;
using Microsoft.JSInterop;

namespace Blazelike.Pages;

public partial class Index : IDisposable
{
    private DotNetObjectReference<Index>? dotNetHelper;

    public Index()
    {
        Board = new Entity[Width, Height];
        for (var i = 0; i < Width; i++)
        {
            Entities.Add(new Entity("Wall", i, 0, "wall", "#", false));
            Entities.Add(new Entity("Wall", i, Height - 1, "wall", "#", false));
            Entities.Add(new Entity("Wall", 0, i, "wall", "#", false));
            Entities.Add(new Entity("Wall", Width - 1, i, "wall", "#", false));
        }
        Player = new Entity("You", Width / 2, Height / 2, "person", "@", false);
        Entities.Add(Player);
    }

    public Entity?[,] Board { get; set; }
    public int Height { get; set; } = 11;
    public string HolderClass { get; set; } = "";
    public bool Loadad { get; private set; }
    public int Width { get; set; } = 11;
    private List<Entity> Entities { get; set; } = new();
    private List<string> Log { get; set; } = new();
    private Entity Player { get; set; }

    public void AddLog(string log)
    {
        Log.Insert(0, log);
        var count = Log.Count;
        if (count > 30)
        {
            Log.RemoveAt(count - 1);
        }
        UpdateBoard();
    }

    public void Dispose()
    {
        dotNetHelper?.Dispose();
    }

    [JSInvokable]
    public void InvokeMove(int keyCode)
    {
        var key = (char)keyCode;
        var x = 0;
        var y = 0;
        var isMovementPressed = false;
        if (key == 'W')
        {
            y--;
            isMovementPressed = true;
        }
        if (key == 'S')
        {
            y++;
            isMovementPressed = true;
        }
        if (key == 'A')
        {
            x--;
            isMovementPressed = true;
        }
        if (key == 'D')
        {
            x++;
            isMovementPressed = true;
        }
        if (!isMovementPressed)
        {
            return;
        }
        MoveBy(x, y);
    }

    public void MoveBy(int x, int y)
    {
        var nextX = Player.Position.X + x;
        var nextY = Player.Position.Y + y;
        if (nextX < 0 || nextX >= Width || nextY < 0 || nextY >= Height)
        {
            return;
        }
        var foundSomething = Entities.FirstOrDefault(e => e.Position == (nextX, nextY) && !e.Walkable);
        if (foundSomething is not null)
        {
            AddLog($"{Player.Name} can't move to {nextX}, {nextY}, because there is a {foundSomething.Name}");
            return;
        }
        Player.SetPosition(nextX, nextY);
        AddLog($"{Player.Name} moved to {nextX}, {nextY}");
        UpdateBoard();
    }

    public void MoveTo(int x, int y)
    {
        var amountX = x - Player.Position.X;
        var amountY = y - Player.Position.Y;
        if (amountX * amountX + amountY * amountY != 1)
        {
            return;
        }
        MoveBy(amountX, amountY);
    }

    public void ToggleRetroMode()
    {
        if (HolderClass is "")
        {
            HolderClass = "retro-mode";
            return;
        }
        HolderClass = "";
    }

    public async Task TriggerDotNetInstanceMethod()
    {
        dotNetHelper = DotNetObjectReference.Create(this);
        await JS.InvokeAsync<string>("init", dotNetHelper);
    }

    protected override async Task OnInitializedAsync()
    {
        UpdateBoard();
        await TriggerDotNetInstanceMethod();
    }

    private void UpdateBoard()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                Board[x, y] = null;
            }
        }
        foreach (var entity in Entities)
        {
            var (x, y) = entity.Position;
            Board[x, y] = entity;
        }
        Loadad = true;
        StateHasChanged();
    }
}