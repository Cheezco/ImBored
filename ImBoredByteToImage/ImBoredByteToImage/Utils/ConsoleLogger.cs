using ImBoredByteToImage.Enums;
using ImBoredByteToImage.Interfaces;

namespace ImBoredByteToImage.Utils;

public class ConsoleLogger : ILogger
{
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public LogLevel LogLevel { get; set; } = LogLevel.Error;

    public void Log(string message, LogLevel logLevel)
    {
        if (!LogLevel.HasFlag(logLevel)) return;
        
        Console.WriteLine($"{Enum.GetName(logLevel)}: {message}");
    }
}