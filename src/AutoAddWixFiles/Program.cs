using Rhyous.AutoAddDLLtoWXSFiles;
using Rhyous.AutoAddDLLtoWXSFiles.Arguments;
using Rhyous.AutoAddDLLtoWXSFiles.TFS;
using Rhyous.SimpleArgs;
using Rhyous.StringLibrary;

new ArgsManager<ArgsHandler>().Start(args);
var wixprojFileList = Args.Value("FL");

var settings = new Settings();
settings.Dll = Args.Value("D");
settings.PrototypeDlls = Args.Value("PD").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToHashSet();
settings.DoNothing = Args.Value("DoNothing").To<bool>();
settings.TFPath = Args.Value("TFdotExePath").To<string>();
settings.CheckoutFromTFS = Args.Value("TFSCheckout").To<bool>();
settings.ExcludeDirs = Args.Value("ExcludeDirectories")?.Split(';', StringSplitOptions.RemoveEmptyEntries)?.ToHashSet();

var files = File.ReadAllLines(wixprojFileList);
List<string> dirs = files.Select(f => Path.GetDirectoryName(f)).ToList();

var finder = new WxsFileFinder(settings);
var potentialWixFiles = finder.FindPotentialWxsFiles(dirs);


if (settings.DoNothing)
{
    foreach (var file in files)
        Console.WriteLine(file);
}
if (!settings.DoNothing && settings.CheckoutFromTFS)
{
    var tfsCheckout = new TFSCheckout(settings);
    tfsCheckout.Checkout(files);
}

var wxsDllFileAdder = new WxsDllFileAdder(settings);
foreach (var wxsFile in potentialWixFiles)
{
    await wxsDllFileAdder.AddAsync(wxsFile);
}