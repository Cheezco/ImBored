using System.Drawing;
using System.Security.Cryptography;
using System.Text.Json;
using ImBoredByteToImage.SaveObjects;

namespace ImBoredByteToImage.BrushTables;

public class OldBrushTable
{
    public Dictionary<SolidBrush, byte> Brushes { get; private set; }

    private OldBrushTable()
    {
        Brushes = new Dictionary<SolidBrush, byte>();
    }
    
    public void Generate(int count)
    {
        Brushes = new Dictionary<SolidBrush, byte>(count);
        for (var i = 0; i < count; i++)
        {
            var red = RandomNumberGenerator.GetInt32(0, 256);
            var green = RandomNumberGenerator.GetInt32(0, 256);
            var blue = RandomNumberGenerator.GetInt32(0, 256);
            var color = Color.FromArgb(255, red, green, blue);
            if (Brushes.Any(x =>
                    x.Key.Color.R == color.R &&
                    x.Key.Color.G == color.G &&
                    x.Key.Color.B == color.B))
            {
                i--;
                continue;
            }

            Brushes.Add(new SolidBrush(color), (byte)i);
        }
    }
    

    public void Save(string path)
    {
        var colors = new List<ColorSave>(Brushes.Count);
        colors.AddRange(Brushes.Select(x => 
            new ColorSave
            {
                Value = x.Value,
                R = x.Key.Color.R,
                G = x.Key.Color.G,
                B = x.Key.Color.B
            }));

        var output = JsonSerializer.Serialize(colors, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(path, output);
    }

    public static OldBrushTable Load(string path)
    {
        var brushTable = new OldBrushTable();
        
        if (!File.Exists(path)) return brushTable;

        var input = File.ReadAllText(path);
        var colors = JsonSerializer.Deserialize<List<ColorSave>>(input);

        if (colors is null) return brushTable;

        brushTable.Brushes = new Dictionary<SolidBrush, byte>();

        foreach (var colorSave in colors)
        {
            brushTable.Brushes.Add(new SolidBrush(colorSave.ToColor()), colorSave.Value);
        }

        return brushTable;
    }
}