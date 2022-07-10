﻿using Blazelike.Game;
using Blazelike.Game.Maps;
using Microsoft.JSInterop;

namespace Blazelike.Pages;

public partial class Index : IDisposable
{
    private DotNetObjectReference<Index>? dotNetHelper;

    public Index()
    {
    }

    public Entity?[,] Board => World.CurrentMap.Board;
    public int Height => World.Height;
    public string HolderClass { get; set; } = "";
    public bool Loadad { get; private set; }
    public List<string> Log => World.Log;
    public int MapHeight => World.CurrentMap.Height;
    public int MapWidth => World.CurrentMap.Width;
    public int Width => World.Width;
    private World World { get; set; } = new();

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
        World.MoveBy(x, y);
        StateHasChanged();
    }

    public void MoveTo(int x, int y)
    {
        var baseX = x - World.Player.Position.X;
        var baseY = y - World.Player.Position.Y;
        var amountX = Math.Sign(baseX);
        var amountY = Math.Sign(baseY);
        if (Math.Abs(baseX) > Math.Abs(baseY))
        {
            amountY = 0;
        }
        else
        {
            amountX = 0;
        }

        if (amountX * amountX + amountY * amountY != 1)
        {
            return;
        }
        World.MoveBy(amountX, amountY);
        StateHasChanged();
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
        World.Update();

        Loadad = true;
        StateHasChanged();
    }
}