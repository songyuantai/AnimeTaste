namespace AnimeTaste.Core.Utils
{
    public static class Logger
    {
        private static readonly Lock _lock = new();
        private static readonly string _baseLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");


        public static void Info(string message)
        {
            Log("INFO", message);
        }

        public static void Error(string message)
        {
            Log("ERROR", message);
        }

        public static void Warning(string message)
        {
            Log("WARNING", message);
        }

        public static void Debug(string message)
        {
            Log("DEBUG", message);
        }

        public static void Exception(Exception ex)
        {
            var errorInfo = "异常信息：" + ex.Message + Environment.NewLine
                + "错误源：" + ex.Source + Environment.NewLine
                + "运行类型：" + ex.GetType() + Environment.NewLine
                + "异常函数：" + ex.TargetSite + Environment.NewLine
                + "堆栈信息：" + ex.StackTrace;
            Log("EXCEPTION", errorInfo);
        }

        private static void Log(string level, string message)
        {
            using (_lock.EnterScope())
            {
                DateTime now = DateTime.Now;
                var logDirectory = Path.Combine(_baseLogPath, level.ToLower(), now.ToString("yyyy-MM"));
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                var logfilePath = Path.Combine(logDirectory, $"{now:yyyy-MM-dd}.{level.ToLower()}.log");
                try
                {
                    string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{Thread.CurrentThread.ManagedThreadId}] {message}{Environment.NewLine}";
                    File.AppendAllText(logfilePath, logEntry);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write log: {ex.Message}");
                }
            }
        }
    }
}