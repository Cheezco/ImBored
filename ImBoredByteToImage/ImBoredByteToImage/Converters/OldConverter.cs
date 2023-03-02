using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ImBoredByteToImage.BrushTables;
using ImBoredByteToImage.Utils;

namespace ImBoredByteToImage.Converters;

public class OldConverter
{
    public bool LogOutput { get; init; }
    
    private const int MaxWidth = 51200;
    private const int MaxHeight = 51200;

    public void ToImage(string inputPath, string outputPath, OldBrushTable oldBrushTable)
    {
        var bytes = ReadFile(inputPath);

        var (width, height) = GetSize(bytes);
        
        using var image = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(image);

        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.FillRectangle(Brushes.White, 0, 0, width, height);

        var xOffset = 0;
        var yOffset = 0;
        var stopwatch = new Stopwatch();
        var flag = true;

        for (var i = 0; i < bytes.Length; i++)
        {
            if (flag)
            {
                stopwatch.Restart();
                flag = false;
            }

            graphics.FillRectangle(oldBrushTable.Brushes.ElementAt(bytes[i]).Key, xOffset, yOffset, 1, 1);
            xOffset++;
            if (xOffset >= width)
            {
                xOffset = 0;
                yOffset++;
            }
            
            stopwatch.Stop();
            var timeLeft = stopwatch.Elapsed.TotalSeconds * (bytes.Length - i);
            ConsoleUtils.WriteLine($"Writing bytes: {Math.Ceiling((i + 1) * 1f / bytes.Length * 100)}% ETA: {Math.Ceiling(timeLeft)}s.", LogOutput);
        }
        ConsoleUtils.WriteLine("Finished writing bytes.", LogOutput);
        ConsoleUtils.WriteLine("Saving image.", LogOutput);

        var img = Optimize(image);

        img.Save(outputPath, ImageFormat.Webp);

        ConsoleUtils.WriteLine("Image saved.", LogOutput);
    }

    public void FromImage(string inputPath, string outputPath, OldBrushTable oldBrushTable)
    {
        var bytes = new List<byte>();

        using var image = Image.FromFile(inputPath) as Bitmap;

        if (image is null) return;

        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                var color = image.GetPixel(x, y);
                
                if(IsColorWhite(color)) break;

                var foundValue = oldBrushTable.Brushes
                    .FirstOrDefault(kvp => kvp.Key.Color.Equals(color));
                
                ConsoleUtils.WriteLine($"Found byte: {foundValue.Value}", LogOutput);
                bytes.Add(foundValue.Value);
            }
        }

        File.WriteAllBytes(outputPath, bytes.ToArray());
        var a= bytes.Select(x => x.ToString())
            .Aggregate((a, b) => $"{a},{b}");
        File.WriteAllText("Test.txt", a);
    }

    private static Bitmap Optimize(Bitmap bitmap)
    {
        var height = 0;
        for (var i = 0; i < bitmap.Height; i++)
        {
            if (IsColorWhite(bitmap.GetPixel(0, i)))
            {
                break;
            }

            height++;
        }
        
        return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, height), bitmap.PixelFormat);
    }

    private static bool IsColorWhite(Color color)
    {
        return color.R == Color.White.R &&
               color.G == Color.White.G &&
               color.B == Color.White.B;
    }


    private (int width, int height) GetSize(IReadOnlyCollection<byte> bytes)
    {
        var width = 24;
        var height = 24;

        while (width * height < bytes.Count &&
               width < MaxWidth &&
               height < MaxHeight)
        {
            width *= 2;
            height *= 2;

            ConsoleUtils.WriteLine($"Invalid size. width: {width} height: {height}", LogOutput);
        }

        if (width * height < bytes.Count)
        {
            throw new Exception("Failed to find correct size.");
        }

        ConsoleUtils.WriteLine($"Found size. width: {width} height: {height}", LogOutput);
        return (width, height);
    }

    private byte[] ReadFile(string path)
    {
        if (!File.Exists(path))
        {
            ConsoleUtils.WriteLine("File does not exist.", LogOutput);
            return Array.Empty<byte>();
        }
        
        ConsoleUtils.WriteLine("Loading data file.", LogOutput);

        var bytes = File.ReadAllBytes(path);

        ConsoleUtils.WriteLine("Data file loaded.", LogOutput);

        return bytes;
    }
}