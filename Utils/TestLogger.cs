using System;
using System.IO;

namespace TestesAutomatizados.Utils
{
    public class TestLogger
    {
        private readonly string _logPath;

        public TestLogger(string logPath)
        {
            _logPath = logPath ?? throw new ArgumentNullException(nameof(logPath)); // Garante não nulo
            Directory.CreateDirectory(Path.GetDirectoryName(_logPath) ?? ".");
        }

        public void Log(string message)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";
            Console.WriteLine(logEntry);
            File.AppendAllText(_logPath, logEntry + Environment.NewLine);
        }
    }
}