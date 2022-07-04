using Microsoft.JSInterop;

namespace Blazelike.Pages;

public partial class Index : IDisposable
{
    private DotNetObjectReference<Index>? dotNetHelper;
    public string[,] Board { get; set; }
    public int Height { get; set; } = 11;
    public string HolderClass { get; set; }
    public int Width { get; set; } = 11;
    private List<string> Log { get; set; } = new();
    private Dictionary<(int X, int Y), string> Map { get; set; } = new();
    private (int X, int Y) PlayerPosition { get; set; } = (4, 4);

    public void AddLog(string log)
    {
        Log.Insert(0, log);
        var count = Log.Count;
        if (count > 30)
        {
            Log.RemoveAt(count - 1);
        }
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
        Board[PlayerPosition.X, PlayerPosition.Y] = "";
        var nextX = PlayerPosition.X + x;
        var nextY = PlayerPosition.Y + y;
        if (nextX < 0 || nextX >= Width || nextY < 0 || nextY >= Height)
        {
            return;
        }
        PlayerPosition = new(nextX, nextY);
        AddLog($"Moved to {nextX}, {nextY}");
        UpdateBoard();
    }

    public void MoveTo(int x, int y)
    {
        var amountX = x - PlayerPosition.X;
        var amountY = y - PlayerPosition.Y;
        if (amountX * amountX + amountY * amountY != 1)
        {
            return;
        }
        MoveBy(amountX, amountY);
    }

    public void ToggleRetroMode()
    {
        if (HolderClass is null)
        {
            HolderClass = "retro-mode";
            return;
        }
        HolderClass = null;
    }

    public async Task TriggerDotNetInstanceMethod()
    {
        dotNetHelper = DotNetObjectReference.Create(this);
        await JS.InvokeAsync<string>("init", dotNetHelper);
    }

    protected override async Task OnInitializedAsync()
    {
        Board = new string[Width, Height];
        for (var i = 0; i < Width; i++)
        {
            Map[(i, 0)] = "#";
            Map[(i, Height - 1)] = "#";
            Map[(0, i)] = "#";
            Map[(Width - 1, i)] = "#";
        }
        UpdateBoard();
        await TriggerDotNetInstanceMethod();
    }

    private void UpdateBoard()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                Board[x, y] = "";
            }
        }
        foreach (var kv in Map)
        {
            var (x, y) = kv.Key;
            Board[x, y] = kv.Value;
        }
        Board[PlayerPosition.X, PlayerPosition.Y] = "@";

        StateHasChanged();
    }
}