
using FileSyncTool.Interfaces;
using FileSyncTool.Utilities;

namespace FileSyncTool.Services;

public class SyncService
{
    private readonly ILogger _logger;
    private readonly Synchronizer _synchronizer;
    private readonly int _intervalSeconds;
    private CancellationTokenSource _cts = new();

    public SyncService(string sourcePath, string replicaPath, string logFilePath, int intervalSeconds)
    {
        ValidatePaths(sourcePath, replicaPath, logFilePath);

        _logger = new Logger(logFilePath);
        _synchronizer = new Synchronizer(sourcePath, replicaPath, _logger);
        _intervalSeconds = intervalSeconds;
    }

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
                    // Gracefully handle Ctrl+C during the delay
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


    public void Stop() => _cts.Cancel();

    private void SetupCancellation()
    {
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            Stop();
        };
    }

    private static void ValidatePaths(string sourcePath, string replicaPath, string logFilePath)
    {
        if (!Directory.Exists(sourcePath))
            throw new DirectoryNotFoundException(string.Format(LogMessages.DirectoryNotFound, sourcePath));

        Directory.CreateDirectory(replicaPath);
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);
    }
}
