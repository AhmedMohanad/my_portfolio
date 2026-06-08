using Portofolio.LoggingServices;

public class SimpleLogger : ISimpleLogger
{
    private readonly string _logFilePath;
    private static readonly object _lock = new(); // one lock for all threads

    public SimpleLogger(IWebHostEnvironment env)
    {
        _logFilePath = Path.Combine(env.ContentRootPath, "logs.txt");
    }

    private void WriteToFile(string message)
    {
        lock (_lock) // only one thread writes at a time
        {
            File.AppendAllText(_logFilePath, message + Environment.NewLine);
        }
    }

    public void LogInfo(string message)
    {
        var log = $"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        Console.WriteLine(log);
        WriteToFile(log);
    }

    public void LogError(string message)
    {
        var log = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(log);
        Console.ResetColor();
        WriteToFile(log);
    }

    public void LogError(Exception ex, string message)
    {
        var log = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message} | {ex.Message}";
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(log);
        Console.ResetColor();
        WriteToFile(log);
    }

    public void LogWarning(string message)
    {
        var log = $"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(log);
        Console.ResetColor();
        WriteToFile(log);
    }

    public void LogSuccess(string message)
    {
        var log = $"[SUCCESS] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(log);
        Console.ResetColor();
        WriteToFile(log);
    }
}