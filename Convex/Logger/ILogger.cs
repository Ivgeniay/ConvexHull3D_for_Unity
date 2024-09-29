namespace MvConvex
{
    public interface ILogger
    {
        public void LogInformation(params object[] message);
        public void LogError(params object[] message);
        public void LogWarning(params object[] message);
        public void LogDebug(params object[] message);
        public void LogTrance(params object[] message);
        public void LogCritical(params object[] message);
        public void Log(LogLevel logLevel, string message);
        public bool IsEnabled(LogLevel logLevel);
    }
}
