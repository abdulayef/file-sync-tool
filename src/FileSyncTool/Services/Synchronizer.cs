

using FileSyncTool.Interfaces;
using System.Security.Cryptography;

namespace FileSyncTool.Services;

public class Synchronizer
{

    private readonly string _sourcePath;
    private readonly string _replicaPath;
    private readonly ILogger _logger;

    public Synchronizer(string sourcePath, string replicaPath, ILogger logger)
    {
        _sourcePath = sourcePath;
        _replicaPath = replicaPath;
        _logger = logger;
    }


    public void Run()
    { }

        private void SyncDirectories(string sourceDir, string replicaDir)
    { }

    private void DeleteOrphans(string sourceDir, string replicaDir)
    { }

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
}