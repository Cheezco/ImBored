using ImBoredByteToImage.Enums;

namespace ImBoredByteToImage.Interfaces;

public interface ILogger
{
    public void Log(string message, LogLevel level = LogLevel.Info);
}