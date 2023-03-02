namespace ImBoredByteToImage.Utils;

public static class ConsoleUtils
{
    public static void WriteLine(string? value, bool write = true)
    {
        if (!write) return;
        
        Console.WriteLine(value);
    }
}