using System.Drawing;

namespace ImBoredByteToImage.SaveObjects;

public readonly struct ColorSave
{
    public byte Value { get; init; }
    public int R { get; init; }
    public int G { get; init; }
    public int B { get; init; }

    public Color ToColor() => Color.FromArgb(255, R, G, B);

    public static ColorSave FromColor(Color color, byte value = 0) =>
        new()
        {
            R = color.R,
            G = color.G,
            B = color.B,
            Value = value
        };
}