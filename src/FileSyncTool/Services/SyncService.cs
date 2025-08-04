
using FileSyncTool.Interfaces;
using FileSyncTool.Utilities;

namespace FileSyncTool.Services;

/// <summary>
/// Service for continuous directory synchronization with interval-based execution and graceful cancellation.
/// </summary>
public class SyncService
{
    private readonly ILogger _logger;
    private readonly Synchronizer _synchronizer;
    private readonly int _intervalSeconds;
    private CancellationTokenSource _cts = new();

    /// <summary>
    /// Initializes a new SyncService instance with validation for all paths.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException">Thrown if source directory doesn't exist.</exception>
    public SyncService(string sourcePath, string replicaPath, string logFilePath, int intervalSeconds)
    {
        ValidatePaths(sourcePath, replicaPath, logFilePath);

        _logger = new Logger(logFilePath);
        _synchronizer = new Synchronizer(sourcePath, replicaPath, _logger);
        _intervalSeconds = intervalSeconds;
    }


    /// <summary>
    /// Starts continuous synchronization with the configured interval.
    /// Handles cancellation (Ctrl+C) and logs fatal errors.
    /// </summary>
    public void Start()
    {
        SetupCancellation();
        try
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                _synchronizer.Run();
                try
                {
                    Task.Delay(_intervalSeconds * 1000, _cts.Token).Wait();
                }
                catch (AggregateException ae) when (ae.InnerException is TaskCanceledException)
                {
                    _logger.Info(LogMessages.SyncStoppedByUser);
                    return;
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.Info(LogMessages.SyncStoppedByUser);
        }
        catch (Exception ex)
        {
            _logger.Info(string.Format(LogMessages.FatalSyncError, ex.Message));
        }

    }

    /// <summary>
    /// Triggers graceful shutdown of the synchronization loop.
    /// </summary>
    public void Stop() => _cts.Cancel();

    private void SetupCancellation()
    {
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            Stop();
        };
    }

    /// <summary>
    /// Validates paths and ensures required directories exist.
    /// </summary>
    private static void ValidatePaths(string sourcePath, string replicaPath, string logFilePath)
    {
        if (!Directory.Exists(sourcePath))
            throw new DirectoryNotFoundException(string.Format(LogMessages.DirectoryNotFound, sourcePath));

        Directory.CreateDirectory(replicaPath);
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);
    }
}
