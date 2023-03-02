namespace ImBoredByteToImage.Enums;

[Flags]
public enum LogLevel
{
    // ReSharper disable once UnusedMember.Global
    None = 0,
    Debug = 1 << 0,
    Info = 1 << 1,
    Warning = 1 << 2,
    Error = 1 << 3,
    All = Info | Warning | Error,
    AllDebug = All | Debug
}