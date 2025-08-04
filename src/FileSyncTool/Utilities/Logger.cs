
using FileSyncTool.Interfaces;
using System.Runtime.CompilerServices;

namespace FileSyncTool.Utilities;

public class Logger : ILogger
{
    private readonly string _logFilePath;
    private readonly object _fileLock = new();

    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;
        EnsureLogDirectoryExists();
        if (!File.Exists(_logFilePath))
            File.Create(_logFilePath).Dispose();
    }

    public void Info(string message, [CallerMemberName] string source = "")
    {
        WriteLog("INFO", message, null, source);
    }

    public void Error(string message, Exception? ex = null, [CallerMemberName] string source = "")
    {
        WriteLog("ERROR", message, ex, source);
    }

    private void WriteLog(string level, string message, Exception? ex, string source)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] [{source}] {message}";

        if (ex != null)
        {
            logEntry += $"\nException: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
        }

        lock (_fileLock)
        {
            try
            {
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception fileEx)
            {
                Console.WriteLine($"!! FAILED TO LOG: {fileEx.Message} !!");
            }
        }

        Console.WriteLine(logEntry);
    }

    private void EnsureLogDirectoryExists()
    {
        var logDir = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }
    }
}