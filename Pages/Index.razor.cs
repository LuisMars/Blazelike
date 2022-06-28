using Microsoft.JSInterop;

namespace Blazelike.Pages;

public partial class Index : IDisposable
{
    private DotNetObjectReference<Index>? dotNetHelper;
    public string[,] Board { get; set; }
    public int Height { get; set; } = 11;
    public int Width { get; set; } = 11;
    private Dictionary<(int X, int Y), string> Map { get; set; } = new();
    private (int X, int Y) PlayerPosition { get; set; } = (4, 4);

    public void Dispose()
    {
        dotNetHelper?.Dispose();
    }

    [JSInvokable]
    public void Move(int key)
    {
        //W = 87
        //S = 83
        //A = 65
        //D = 68
        var x = 0;
        var y = 0;
        var isMovementPressed = false;
        if (key == 87)
        {
            y--;
            isMovementPressed = true;
        }
        if (key == 83)
        {
            y++;
            isMovementPressed = true;
        }
        if (key == 65)
        {
            x--;
            isMovementPressed = true;
        }
        if (key == 68)
        {
            x++;
            isMovementPressed = true;
        }
        if (!isMovementPressed)
        {
            return;
        }
        Board[PlayerPosition.X, PlayerPosition.Y] = "";
        var nextX = PlayerPosition.X + x;
        var nextY = PlayerPosition.Y + y;
        if (nextX < 0 || nextX >= Width || nextY < 0 || nextY >= Height)
        {
            return;
        }
        PlayerPosition = new(nextX, nextY);
        UpdateBoard();
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