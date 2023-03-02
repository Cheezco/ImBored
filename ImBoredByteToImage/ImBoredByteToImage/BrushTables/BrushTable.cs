using System.Drawing;
using System.Security.Cryptography;
using System.Text.Json;
using ImBoredByteToImage.Extensions;
using ImBoredByteToImage.Interfaces;
using ImBoredByteToImage.SaveObjects;

namespace ImBoredByteToImage.BrushTables;

public class BrushTable : IBrushTable
{
    public SolidBrush this[byte value]
    {
        get
        {
            var found = _brushTable.FirstOrDefault(x => x.Value == value);

            return found.Key;
        }
    }

    public byte this[SolidBrush brush]
    {
        get
        {
            var found = _brushTable.FirstOrDefault(x =>
                x.Key.Equals(brush));

            return found.Value;
        }
    }

    public byte this[Color color]
    {
        get
        {
            var found = _brushTable.FirstOrDefault(x =>
                x.Key.Color.IsColor(color));

            return found.Value;
        }
    }

    private Dictionary<SolidBrush, byte> _brushTable = new();

    public void Generate(int count)
    {
        if (count is < 0 or > byte.MaxValue + 1) return;
        
        _brushTable = new Dictionary<SolidBrush, byte>(count);
        for (var i = 0; i < count; i++)
        {
            var red = RandomNumberGenerator.GetInt32(0, 256);
            var green = RandomNumberGenerator.GetInt32(0, 256);
            var blue = RandomNumberGenerator.GetInt32(0, 256);
            var color = Color.FromArgb(255, red, green, blue);

            if (_brushTable.Any(x => x.Key.Color.Equals(color)))
            {
                i--;
                continue;
            }

            _brushTable.Add(new SolidBrush(color), (byte)i);
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public void Save(string path)
    {
        var colors = new List<ColorSave>(_brushTable.Count);
        colors.AddRange(_brushTable.Select(x =>
            ColorSave.FromColor(x.Key.Color, x.Value)));

        var output = JsonSerializer.Serialize(colors, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(path, output);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static BrushTable Load(string path)
    {
        var brushTable = new BrushTable();

        if (!File.Exists(path)) return brushTable;

        var input = File.ReadAllText(path);
        var colors = JsonSerializer.Deserialize<List<ColorSave>>(input);
        
        if(colors is null) return brushTable;

        foreach (var colorSave in colors)
        {
            brushTable._brushTable.Add(new SolidBrush(colorSave.ToColor()), colorSave.Value);
        }

        return brushTable;
    }

    public static BrushTable CreateIfMissing(string path, int count = 256)
    {
        BrushTable table;
        
        if (!File.Exists(path))
        {
            table = new BrushTable();
            table.Generate(count);
            table.Save(path);
        }
        else
        {
            table = Load(path);
        }

        return table;
    }
}