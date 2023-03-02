using ImBoredByteToImage;
using ImBoredByteToImage.BrushTables;
using ImBoredByteToImage.Converters;
using ImBoredByteToImage.Enums;
using ImBoredByteToImage.Utils;

// const string inputFile = "testData/testData.json";
// const string inputFile = "testData/testData.xlsx";
const string inputFile = "testData/beeMovieScript.txt";
// const string inputFile = "testData/Cat03.jpg";
// const string inputFile = "testData/largeText.txt";


const string outputFile = "testDataImage.webp";


// const string output2File = "testDataTest.json";
// const string output2File = "testDataTest.xlsx";
const string output2File = "beeMovieScriptOutput.txt";
// const string output2File = "largeTextOutput.txt";
// const string output2File = "Cat03Output.jpg";


const bool runOldConverter = false;
var logger = new ConsoleLogger
{
    LogLevel = LogLevel.AllDebug
};

if (runOldConverter)
#pragma warning disable CS0162
{
    var runner = new OldConverterRunner(logger);

    runner.RunToImage(inputFile, outputFile);
    // runner.RunFromImage(outputFile, output2File);
}
#pragma warning restore CS0162
else
{
    var table = BrushTable.CreateIfMissing("testTable.json");
    var converter = new Converter(table)
    {
        Logger = logger
    };

    var runner = new ConverterRunner(converter, logger);

    runner.RunToImage(inputFile, outputFile);
    // runner.RunFromImage(outputFile, output2File);
}

