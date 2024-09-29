using System.Text;
using UnityEngine;

namespace MvConvex
{
    public class UnitySimpleLogger : ILogger
    {
        public void LogTrance(params object[] message) => Log(LogLevel.Information, ObjectsToStringConcat(message));
        public void LogDebug(params object[] message) => Log(LogLevel.Debug, ObjectsToStringConcat(message));
        public void LogInformation(params object[] message) => Log(LogLevel.Information, ObjectsToStringConcat(message));
        public void LogWarning(params object[] message) => Log(LogLevel.Warning, ObjectsToStringConcat(message));
        public void LogError(params object[] message) => Log (LogLevel.Error, ObjectsToStringConcat(message));
        public void LogCritical(params object[] message) => Log (LogLevel.Error, ObjectsToStringConcat(message));

        private string ObjectsToStringConcat(params object[] message)
        {
            StringBuilder sb = new StringBuilder(); 
            foreach (object obj in message)
            {
                sb.Append(obj.ToString());
            }
            return sb.ToString();
        }
        public void Log(LogLevel logLevel, string message)
        {
            string coloredLogLevelString = string.Empty;
            switch (logLevel)
            {
                case LogLevel.Trace:
                    coloredLogLevelString = $"<color=grey>{logLevel}</color>";
                    break;
                case LogLevel.Debug:
                    coloredLogLevelString = $"<color=blue>{logLevel}</color>";
                    break;
                case LogLevel.Information:
                    coloredLogLevelString = $"<color=white>{logLevel}</color>";
                    break;
                case LogLevel.Warning:
                    coloredLogLevelString = $"<color=yellow>{logLevel}</color>";
                    break;
                case LogLevel.Error:
                    coloredLogLevelString = $"<color=red>{logLevel}</color>";
                    break;
                case LogLevel.Critical:
                    coloredLogLevelString = $"<color=orange>{logLevel}</color>"; 
                    break;
            }
            Debug.Log($"[{coloredLogLevelString}] {message}");
        }

        public bool IsEnabled(LogLevel logLevel) => true;
    }
}
