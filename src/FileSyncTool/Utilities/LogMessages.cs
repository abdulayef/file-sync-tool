namespace FileSyncTool.Utilities;

public static class LogMessages
{
    // Sync process
    public const string SyncStarted = "Starting synchronization...";
    public const string SyncComplete = "Synchronization complete.";
    public const string SyncFailed = "Synchronization failed for {0}: {1}";
    public const string SyncStoppedByUser = "Synchronization stopped by user request";

    // File operations
    public const string FileCopied = "Copied: {0}";
    public const string FileDeleted = "Deleted: {0}";
    public const string FileSkipped = "Skipped {0} (in use/missing): {1}";

    // Directories
    public const string DirCreated = "Created directory: {0}";
    public const string DirDeleted = "Deleted folder: {0}";
    public const string DirNotFound = "Directory not found: {0}";
    public const string DirectoryNotFound = "Directory not found: {0}";

    // Errors
    public const string FatalError = "Fatal error: {0}";
    public const string ConfigError = "Configuration error: {0}";
    public const string FatalSyncError = "Fatal sync error: {0}";
    public const string FileCopyFailed = "Failed to copy {0}: {1}";
    public const string InvalidArgs = "Usage: FolderSyncTool <sourcePath> <replicaPath> <intervalInSeconds> <logFilePath> or no arguments for config.json";
    public const string ConfigFileNotFound = "Config file not found";
    public const string InvalidConfig = "Invalid configuration in config file";
    public const string InvalidInterval = "Invalid time interval";

}
