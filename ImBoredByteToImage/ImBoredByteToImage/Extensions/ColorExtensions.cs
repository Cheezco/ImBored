using System.Drawing;

namespace ImBoredByteToImage.Extensions;

public static class ColorExtensions
{
    public static bool IsColor(this Color color, Color other)
    {
        return color.R == other.R && color.G == other.G && color.B == other.B;
    }
}