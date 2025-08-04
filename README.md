# File Sync Tool 🔄
**Professional-grade directory synchronization utility**  
*Built with .NET 6, featuring configurable intervals, robust error handling, and detailed logging*

## ✨ Features
- **One-way synchronization** with MD5 hash verification
- **Configurable sync intervals** (CLI or JSON config)
- **Resilient retry logic** for file operations
- **Detailed logging** with timestamps and error tracking
- **Graceful shutdown** (Ctrl+C handling)

## 🚀 Quick Start
```bash
# Clone and run
git clone https://github.com/abdulayef/file-sync-tool.git
cd file-sync-tool
dotnet run --source ./source_dir --replica ./backup_dir --interval 60
```

## ⚙️ Configuration
### Option 1: Command Line
```bash
dotnet run --source [SOURCE_PATH] --replica [REPLICA_PATH] --interval [SECONDS] --log [LOG_PATH]
```

### Option 2: config.json
```json
{
  "SourcePath": "./source",
  "ReplicaPath": "./backup",
  "IntervalInSeconds": 60,
  "LogFilePath": "./logs/sync.log"
}
```

## 🧪 Testing
```bash
dotnet test  # Runs unit tests
```
**Test Coverage**:  
✅ Error handling scenarios  
✅ Concurrent access edge cases  

## 🛠️ Technical Highlights
- **SOLID Principles**: Clean separation of concerns
- **Dependency Injection**: Mockable services
- **Cross-Platform**: Windows/Linux/macOS support

## 📂 Project Structure
```
src/
├── FileSyncTool/          # Core logic
│   ├── Services/         # Sync implementations
│   └── Utilities/        # Helpers
tests/
└── FileSyncTool.Tests/   # xUnit tests
```