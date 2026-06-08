namespace Portofolio.LoggingServices
{
    public class SimpleLogger : ISimpleLogger
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _logFilePath;

        public SimpleLogger(IWebHostEnvironment env)
        {
            _env = env;
            _logFilePath = Path.Combine(_env.ContentRootPath, "logs.txt");
        }

        public void LogInfo(string message)
        {
            var logMessage = $"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            Console.WriteLine(logMessage);
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }

        public void LogError(string message)
        {
            var logMessage = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(logMessage);
            Console.ResetColor();
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }

        public void LogError(Exception ex, string message)
        {
            var logMessage = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message} - Exception: {ex.Message} - StackTrace: {ex.StackTrace}";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(logMessage);
            Console.ResetColor();
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }

        public void LogWarning(string message)
        {
            var logMessage = $"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(logMessage);
            Console.ResetColor();
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }

        public void LogSuccess(string message)
        {
            var logMessage = $"[SUCCESS] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(logMessage);
            Console.ResetColor();
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }
    }
}

