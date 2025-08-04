
using FileSyncTool.Models;
using FileSyncTool.Utilities;
using System.Text.Json;



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