
using FileSyncTool.Models;
using FileSyncTool.Services;
using FileSyncTool.Utilities;
using System.Text.Json;


/// <summary>
/// Main entry point for the file synchronization tool.
/// Handles configuration loading and service initialization.
/// </summary>
try
{
    // Load configuration from either config file or command line
    var (sourcePath, replicaPath, intervalSeconds, logFilePath) = GetConfiguration(args);

    // Initialize and start the synchronization service
    var syncService = new SyncService(sourcePath, replicaPath, logFilePath, intervalSeconds);
    syncService.Start();
}
catch (Exception ex)
{
    // Display fatal errors in red and exit with error code
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Fatal error: {ex.Message}");
    Console.ResetColor();
    Environment.Exit(1);
}

/// <summary>
/// Determines configuration source (file or args) and returns validated settings
/// </summary>
/// <exception cref="ArgumentException">Thrown for invalid arguments</exception>
/// <exception cref="FileNotFoundException">Thrown if config.json is missing</exception>
static (string sourcePath, string replicaPath, int intervalSeconds, string logFilePath) GetConfiguration(string[] args)
{
    return args.Length switch
    {
        0 => LoadConfigFromFile(), // Use config file when no args provided
        4 => ParseCommandLineArgs(args),  // Use command line args when exactly 4 provided
        _ => throw new ArgumentException(LogMessages.InvalidArgs)
    };
}

/// <summary>
/// Parses and validates configuration from command line arguments
/// </summary>
/// <param name="args">Command line arguments array</param>
/// <exception cref="ArgumentException">Thrown for invalid interval values</exception>
static (string, string, int, string) ParseCommandLineArgs(string[] args)
{
    if (!int.TryParse(args[2], out int interval) || interval <= 0)
        throw new ArgumentException(LogMessages.InvalidInterval);

    return (args[0], args[1], interval, args[3]);
}


/// <summary>
/// Loads and validates configuration from config.json file
/// </summary>
/// <exception cref="FileNotFoundException">Thrown if config.json is missing</exception>
/// <exception cref="InvalidDataException">Thrown for invalid JSON structure</exception>
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