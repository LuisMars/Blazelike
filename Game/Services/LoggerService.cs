namespace Blazelike.Game.Services;

public class LoggerService
{
    public List<string> Log { get; } = new();

    public void AddLog(string message)
    {
        Console.WriteLine(message);
        Log.Insert(0, message);
        var count = Log.Count;
        if (count > 30)
        {
            Log.RemoveAt(count - 1);
        }
    }

    internal void LogConsole(string message)
    {
        Console.WriteLine(message);
    }
}