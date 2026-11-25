using System;
using System.IO;
using System.Diagnostics;

namespace MajorProject.Business.Helpers
{
    public static class Logger
    {
        private static readonly string _logFilePath;
        private static readonly object _lock = new object ();
        static Logger()
        {
            // Log file in the application directory
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            _logFilePath = Path.Combine(appDir, "app_log.txt");
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        public static void Info(string message)
        {
            Log("INFO", message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public static void Warning(string message)
        {
            Log("WARN", message);
        }

        /// <summary>
        /// Logs an error message (including exception details if provided).
        /// </summary>
        public static void Error(string message, Exception ex = null)
        {
            string fullMessage = message;
            if (ex != null)
                fullMessage = $"{message}{Environment.NewLine}Exception: {ex}";
            Log("ERROR", fullMessage);
        }

        /// <summary>
        /// Internal logging method – writes to file and debug output.
        /// </summary>
        private static void Log(string level, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] [{level}] {message}";
            // 1. Write to debug output (Visible in Visual Studio Output window)
            Debug.WriteLine(logEntry);
            // 2. Write to file (with thread-safe locking)
            try
            {
                lock (_lock)
                {
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
            }
            catch
            {
            // If file writing fails, we don't want to crash the app.
            // The debug output still captured the error.
            }
        }
    }
}
