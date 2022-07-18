namespace Blazelike.Game.Actions;

public interface IAction
{
    bool Act();

    bool TryPrepare(int x, int y);
}