namespace Blazelike.Game.Services;

public class LoggerService
{
    public List<string> LogList { get; } = new();

    public void Log(string message)
    {
        Console.WriteLine(message);
        LogList.Insert(0, message);
        var count = LogList.Count;
        if (count > 30)
        {
            LogList.RemoveAt(count - 1);
        }
    }

    internal void LogConsole(string message)
    {
        Console.WriteLine(message);
    }
}