

using FileSyncTool.Interfaces;

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
        return true;
    }
}