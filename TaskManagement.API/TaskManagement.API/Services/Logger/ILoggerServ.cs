namespace TaskManagement.API.Services.Logger
{
    public interface ILoggerServ
    {
        void LogInfo(string message);
        void LogDebug(string message);
        void LogWarn(string message);
        void LogError(string message);
    }
}
