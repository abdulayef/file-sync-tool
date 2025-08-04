
using System.Runtime.CompilerServices;

namespace FileSyncTool.Interfaces;

public interface ILogger
{
    void Info(string message,
             [CallerMemberName] string source = "");

    void Error(string message,
              Exception? ex = null,
              [CallerMemberName] string source = "");
}