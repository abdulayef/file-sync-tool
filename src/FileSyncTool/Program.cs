
using FileSyncTool.Models;
using FileSyncTool.Services;
using FileSyncTool.Utilities;
using System.Text.Json;

try
{
    var (sourcePath, replicaPath, intervalSeconds, logFilePath) = GetConfiguration(args);
    var syncService = new SyncService(sourcePath, replicaPath, logFilePath, intervalSeconds);
    syncService.Start();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Fatal error: {ex.Message}");
    Console.ResetColor();
    Environment.Exit(1);
}


static (string sourcePath, string replicaPath, int intervalSeconds, string logFilePath) GetConfiguration(string[] args)
{
    return args.Length switch
    {
        0 => LoadConfigFromFile(),
        4 => ParseCommandLineArgs(args),
        _ => throw new ArgumentException(LogMessages.InvalidArgs)
    };
}

static (string, string, int, string) ParseCommandLineArgs(string[] args)
{
    if (!int.TryParse(args[2], out int interval) || interval <= 0)
        throw new ArgumentException(LogMessages.InvalidInterval);

    return (args[0], args[1], interval, args[3]);
}
static (string, string, int, string) LoadConfigFromFile()
{
    const string configPath = "config.json";
    if (!File.Exists(configPath))
        throw new FileNotFoundException(LogMessages.ConfigFileNotFound);

    var config = JsonSerializer.Deserialize<SyncConfig>(File.ReadAllText(configPath))
        ?? throw new InvalidDataException(LogMessages.InvalidConfig);

    if (config.IntervalInSeconds <= 0)
        throw new ArgumentException(LogMessages.InvalidInterval);

    return (config.SourcePath, config.ReplicaPath, config.IntervalInSeconds, config.LogFilePath);
}