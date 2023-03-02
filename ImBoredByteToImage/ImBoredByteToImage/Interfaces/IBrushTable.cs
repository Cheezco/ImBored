using System.Drawing;

namespace ImBoredByteToImage.Interfaces;

public interface IBrushTable
{
    public SolidBrush this[byte value] { get; }
    // ReSharper disable once UnusedMember.Global
    public byte this[SolidBrush brush] { get; }
    public byte this[Color color] { get; }
}