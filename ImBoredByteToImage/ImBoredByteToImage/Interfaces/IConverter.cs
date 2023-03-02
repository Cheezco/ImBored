using System.Drawing;

namespace ImBoredByteToImage.Interfaces;

public interface IConverter
{
    public Bitmap ToImage(byte[] data);
    public byte[] FromImage(Bitmap bitmap);
}