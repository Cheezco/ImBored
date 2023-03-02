using System.Drawing;
using ImBoredByteToImage.Enums;
using ImBoredByteToImage.Interfaces;

namespace ImBoredByteToImage;

public class ConverterRunner
{
    private readonly IConverter _converter;

    private readonly ILogger? _logger;
 
    public ConverterRunner(IConverter converter, ILogger? logger = null)
    {
        _converter = converter;
        _logger = logger;
    }
    

    // ReSharper disable once UnusedMember.Global
    public void RunToImage(string inputFile, string outputFile)
    {
        if (!File.Exists(inputFile))
        {
            _logger?.Log("File not found.", LogLevel.Error);
            return;
        }
        
        var data = File.ReadAllBytes(inputFile);

        var bitmap = _converter.ToImage(data);
        
        _logger?.Log("Saving image to file.");

        bitmap.Save(outputFile);
        
        _logger?.Log("Image saved to file.");
    }

    public void RunFromImage(string inputFile, string outputFile)
    {
        if (!File.Exists(inputFile))
        {
            _logger?.Log("File not found.", LogLevel.Error);
            return;
        }

        _logger?.Log("Reading image from file.");

        using var bitmap = Image.FromFile(inputFile) as Bitmap;

        if (bitmap is null)
        {
            _logger?.Log("Failed to get bitmap.", LogLevel.Error);
            return;
        }
        
        _logger?.Log("Finished reading image from file.");

        var data = _converter.FromImage(bitmap);
        
        _logger?.Log("Writing data to file.");
        
        File.WriteAllBytes(outputFile, data);
        
        _logger?.Log("Finished data to file.");
    }
}