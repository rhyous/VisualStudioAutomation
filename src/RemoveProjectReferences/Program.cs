using Rhyous.RemoveProjectReferences.Arguments;
using Rhyous.RemoveProjectReferences;
using Rhyous.RemoveProjectReferences.TFS;
using Rhyous.SimpleArgs;
using Rhyous.StringLibrary;

new ArgsManager<ArgsHandler>().Start(args);
var fileExtension = Args.Value("FE");
var searchPattern = Args.Value("P");
var searchDirectory = Args.Value("SD");
var settings = new Settings();
settings.DoNothing = Args.Value("DoNothing").To<bool>();
settings.TFPath = Args.Value("TFdotExePath").To<string>();
settings.CheckoutFromTFS = Args.Value("TFSCheckout").To<bool>();
settings.ExcludeDirs = Args.Value("ExcludeDirectories")?.Split(';', StringSplitOptions.RemoveEmptyEntries)?.ToList();

var files = new List<string>();
foreach (var csprojPath in Directory.GetFiles(searchDirectory, $"*.{fileExtension}", SearchOption.AllDirectories))
{
    if (File.ReadAllText(csprojPath).Contains(searchPattern))
    {
        files.Add(csprojPath);
    }
}


var tfsCheckout = new TFSCheckout(settings);
if (settings.DoNothing)
{
    foreach (var file in files)
        Console.WriteLine(file);
    return;
}
if (settings.CheckoutFromTFS)
    tfsCheckout.Checkout(files);

foreach (var file in files)
{
    var lines = File.ReadAllLines(file);
    var newFileLines = new List<string>();
    var i = 0;
    var line = lines[i];
    // Write all lines before pattern found
    while (!line.Contains(searchPattern))
    {
        newFileLines.Add(line);
        line = lines[++i];
    }
    // Skip snippet
    while (!line.Contains("</ProjectReference>"))
    {
        line = lines[++i];
    }
    // Skip final end element tag
    line = lines[++i];
    // Add all remaining lines
    while (i < lines.Length)
    {
        newFileLines.Add(line);
        i++;
        if (i < lines.Length)
            line = lines[i];
    }
    File.WriteAllLines(file, newFileLines);
}