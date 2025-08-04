

using FileSyncTool.Interfaces;
using FileSyncTool.Utilities;
using System.Diagnostics;
using System.Security.Cryptography;

namespace FileSyncTool.Services;

public class Synchronizer
{

    private readonly string _sourcePath;
    private readonly string _replicaPath;
    private readonly ILogger _logger;

    public Synchronizer(string sourcePath, string replicaPath, ILogger logger)
    {
        //validation
        if (string.IsNullOrWhiteSpace(sourcePath))
            throw new ArgumentNullException(nameof(sourcePath));
        if (string.IsNullOrWhiteSpace(replicaPath))
            throw new ArgumentNullException(nameof(replicaPath));
        if (logger == null)
            throw new ArgumentNullException(nameof(logger));

        _sourcePath = sourcePath;
        _replicaPath = replicaPath;
        _logger = logger;
    }


    public void Run()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.Info(LogMessages.SyncStarted);

        try
        {
            SyncDirectories(_sourcePath, _replicaPath);
            _logger.Info($"{LogMessages.SyncComplete} ({stopwatch.ElapsedMilliseconds}ms)");
        }
        catch (Exception ex)
        {
            _logger.Error(LogMessages.SyncFailed, ex);
            throw;
        }
    }

    private void SyncDirectories(string sourceDir, string replicaDir)
    {
        try
        {
            // Ensure target directory exists
            if (!Directory.Exists(replicaDir))
            {
                Directory.CreateDirectory(replicaDir);
                _logger.Info(string.Format(LogMessages.DirCreated, replicaDir));
            }

            // Copy and update files
            foreach (var sourceFile in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(sourceFile);
                string replicaFile = Path.Combine(replicaDir, fileName);

                try
                {
                    if (!File.Exists(replicaFile) || !FilesAreEqual(sourceFile, replicaFile))
                    {

                        CopyFileWithMetadata(sourceFile, replicaFile);
                        _logger.Info(string.Format(LogMessages.FileCopied, fileName));
                    }
                }
                catch (IOException ex)
                {
                    _logger.Info(string.Format(LogMessages.FileCopyFailed, fileName, ex.Message));
                }
            }

            // Recursively sync subdirectories
            foreach (var sourceSubDir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(sourceSubDir);
                string replicaSubDir = Path.Combine(replicaDir, dirName);

                SyncDirectories(sourceSubDir, replicaSubDir);
            }

            // Delete files and folders not in source
            DeleteOrphans(sourceDir, replicaDir);
        }
        catch (Exception ex)
        {
            _logger.Info(string.Format(LogMessages.FatalSyncError, ex.Message));
            throw; // Re-throw to allow caller to handle
        }

    }

    private void DeleteOrphans(string sourceDir, string replicaDir)
    {        
        // Delete extra files
        foreach (var replicaFile in Directory.GetFiles(replicaDir))
        {
            string fileName = Path.GetFileName(replicaFile);
            string sourceFile = Path.Combine(sourceDir, fileName);

            if (!File.Exists(sourceFile))
            {
                ExecuteWithRetry(
                    () => File.Delete(replicaFile),
                    $"Delete {Path.GetFileName(replicaFile)}"
                    );
                _logger.Info(string.Format(LogMessages.DirDeleted, fileName));
            }
        }

        // Delete extra folders
        foreach (var replicaSubDir in Directory.GetDirectories(replicaDir))
        {
            string dirName = Path.GetFileName(replicaSubDir);
            string sourceSubDir = Path.Combine(sourceDir, dirName);

            if (!Directory.Exists(sourceSubDir))
            {
                Directory.Delete(replicaSubDir, true);
                _logger.Info(string.Format(LogMessages.DirDeleted, dirName));
            }
            else
            {
                // Recursively check for orphaned files/folders
                DeleteOrphans(sourceSubDir, replicaSubDir);
            }
        }
    }

    private bool FilesAreEqual(string file1, string file2) 
    {
        return GetFileHash(file1) == GetFileHash(file2);
    }
    private string GetFileHash(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        byte[] hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
    private void CopyFileWithMetadata(string source, string target)
    {
        File.Copy(source, target, overwrite: true);
        File.SetCreationTimeUtc(target, File.GetCreationTimeUtc(source));
        File.SetLastWriteTimeUtc(target, File.GetLastWriteTimeUtc(source));
        File.SetAttributes(target, File.GetAttributes(source));
    }

    private void ExecuteWithRetry(Action action, string operationName, int maxRetries = 3, int baseDelayMs = 100)
    {
        int attempt = 0;
        List<Exception> exceptions = new();

        while (attempt < maxRetries)
        {
            try
            {
                attempt++;
                action();
                return;
            }
            catch (IOException ex)
            {
                exceptions.Add(ex);
                if (attempt < maxRetries)
                {
                    int delay = baseDelayMs * (int)Math.Pow(2, attempt); // Exponential backoff
                    _logger.Info($"Attempt {attempt}/{maxRetries} failed for {operationName}. Retrying in {delay}ms...");
                    Thread.Sleep(delay);
                }
            }
        }

        throw new AggregateException(
            $"{operationName} failed after {maxRetries} attempts",
            exceptions
        );
    }
}