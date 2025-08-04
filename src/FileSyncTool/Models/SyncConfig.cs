namespace FileSyncTool.Models;

public class SyncConfig
{
    public string SourcePath { get; set; }
    public string ReplicaPath { get; set; }
    public int IntervalInSeconds { get; set; }
    public string LogFilePath { get; set; }
}
