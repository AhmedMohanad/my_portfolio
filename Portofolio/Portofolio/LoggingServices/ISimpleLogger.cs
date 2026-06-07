namespace Portofolio.LoggingServices
{
    public interface ISimpleLogger
    {
        void LogInfo(string message);
        void LogError(string message);
        void LogError(Exception ex, string message);
        void LogWarning(string message);
        void LogSuccess(string message);
    }
}
