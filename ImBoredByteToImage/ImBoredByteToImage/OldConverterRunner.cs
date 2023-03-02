using ImBoredByteToImage.BrushTables;
using ImBoredByteToImage.Converters;
using ImBoredByteToImage.Enums;
using ImBoredByteToImage.Interfaces;

namespace ImBoredByteToImage;

public class OldConverterRunner
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public bool UseOldLogger { get; set; } = true;
    
    private readonly ILogger? _logger;
    private const bool LoadTableFromFile = true;
    private const bool SaveTableToFile = true;
    private const string TablePath = "testTable.json";
    
    public OldConverterRunner(ILogger? logger = null)
    {
        _logger = logger;
    }
    
    public void RunToImage(string inputFile, string outputFile)
    {
        var table = LoadTable();

        var converter = new OldConverter
        {
            LogOutput = UseOldLogger
        };
        
        converter.ToImage(inputFile, outputFile, table);
    }
    
    // ReSharper disable once UnusedMember.Global
    public void RunFromImage(string inputFile, string outputFile)
    {
        var table = LoadTable();

        var converter = new OldConverter
        {
            LogOutput = UseOldLogger
        };
        
        converter.FromImage(inputFile, outputFile, table);
    }

    private OldBrushTable LoadTable()
    {
        OldBrushTable table;
        
        if (LoadTableFromFile)
        {
            _logger?.Log($"{nameof(LoadTableFromFile)} set to true.", LogLevel.Debug);
            _logger?.Log($"Loading table from file. Path: {TablePath}");
            
            table = OldBrushTable.Load(TablePath);
            
            _logger?.Log("Finished loading table.");
        }
        else
#pragma warning disable CS0162
        {
            _logger?.Log($"{nameof(LoadTableFromFile)} set to false.", LogLevel.Debug);
            _logger?.Log("Generating new table.");
            
            table.Generate(256);

            if (SaveTableToFile)
            {
                _logger?.Log($"{nameof(SaveTableToFile)} set to true.", LogLevel.Debug);
                _logger?.Log("Saving table to file.");
                
                table.Save(TablePath);
                
                _logger?.Log($"Saved table to file. Path: {TablePath}");
            }
        }
#pragma warning restore CS0162

        return table;
    }
}