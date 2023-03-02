using System.Globalization;

namespace ImBoredByteToImage.Utils;

public static class LoggingUtils
{
    public static string GetTimeString(TimeSpan timeSpan)
        => timeSpan.ToString(@"hh\:mm\:ss\:fff");

    public static string GetPercentageString(int currentPosition, int total) 
        => Math.Ceiling((currentPosition + 1) * 1f / total * 100)
            .ToString(CultureInfo.InvariantCulture);
}