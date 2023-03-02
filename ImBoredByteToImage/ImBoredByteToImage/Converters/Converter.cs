using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using ImBoredByteToImage.Enums;
using ImBoredByteToImage.Extensions;
using ImBoredByteToImage.Interfaces;
using ImBoredByteToImage.Utils;

namespace ImBoredByteToImage.Converters;

public class Converter: IConverter
{
    public ILogger? Logger { get; init; }
    
    private readonly IBrushTable _brushTable;
    
    private const int MaxWidth = 51200;
    private const int MaxHeight = 51200;
    private const int ByteSize = 1;

    public Converter(IBrushTable brushTable)
    {
        _brushTable = brushTable;
    }

    public Bitmap ToImage(byte[] data)
    {
        Logger?.Log("Getting image size.");
        
        var (width, height) = GetImageSize(data);
        
        Logger?.Log("Creating blank image.");

        using var image = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(image);
        
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.FillRectangle(Brushes.White, 0, 0, width, height);
        
        Logger?.Log("Created blank image.");

        var xOffset = 0;
        var yOffset = 0;
        var stopwatch = new Stopwatch();
        var firstMeasured = false;
        
        Logger?.Log("Starting to write bytes.");
        
        for (var i = 0; i < data.Length; i++)
        {
            if (!firstMeasured)
            {
                stopwatch.Restart();
                firstMeasured = true;
            }
            
            graphics.FillRectangle(_brushTable[data[i]], xOffset, yOffset, ByteSize, ByteSize);
            xOffset += ByteSize;
            if (xOffset >= width)
            {
                xOffset = 0;
                yOffset += ByteSize;
            }

            stopwatch.Stop();
            var timeLeft = TimeSpan.FromSeconds(stopwatch.Elapsed.TotalSeconds * (data.Length - i));
            Logger?.Log($"Writing bytes: {LoggingUtils.GetPercentageString(i, data.Length)}%" +
                        $" Time left: {LoggingUtils.GetTimeString(timeLeft)}.");
        }
        Logger?.Log("Finished writing bytes.");
        Logger?.Log("Optimizing image height.", LogLevel.Debug);

        var optimizedImage = OptimizeImageHeight(image);

        Logger?.Log($"Finished optimizing image height. Before: {image.Width}x{image.Height}" +
                    $" After: {optimizedImage.Width}x{optimizedImage.Height}", LogLevel.Debug);
        
        return optimizedImage;
    }

    public byte[] FromImage(Bitmap bitmap)
    {
        var bytes = new List<byte>();
        
        Logger?.Log("Started reading bytes from image.");

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);

                if (color.IsColor(Color.White)) break;

                var value = _brushTable[color];
                
                Logger?.Log($"Found byte: {value}", LogLevel.Debug);
                bytes.Add(value);
            }
        }
        
        Logger?.Log("Finished reading bytes from image.");

        return bytes.ToArray();
    }
    
    private static Bitmap OptimizeImageHeight(Bitmap bitmap)
    {
        var height = 0;
        for (var i = 0; i < bitmap.Height; i++)
        {
            if (bitmap.GetPixel(0, i).IsColor(Color.White))
            {
                break;
            }

            height++;
        }
        
        return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, height), bitmap.PixelFormat);
    }
    
    private (int width, int height) GetImageSize(IReadOnlyCollection<byte> data)
    {
        var width = 24;
        var height = 24;

        while (width * height < data.Count &&
               width < MaxWidth &&
               height < MaxHeight)
        {
            width *= 2;
            height *= 2;
            
            Logger?.Log($"Invalid size. width: {width} height: {height}", LogLevel.Debug);
        }

        if (width * height < data.Count)
        {
            Logger?.Log("Failed to find correct size.", LogLevel.Error);
            throw new Exception("Failed to find correct size.");
        }
        
        Logger?.Log($"Found size. width: {width} height: {height}", LogLevel.Debug);
        return (width, height);
    }
}