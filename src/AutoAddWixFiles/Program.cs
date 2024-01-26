using AutoAddWixFiles.Arguments;
using Rhyous.SimpleArgs;
using System.Net.WebSockets;

const string Template = "{0}<File Id=\"{1}\" Name=\"{1}\" Source=\"$(var.{2}.TargetDir){1}\" />";



new ArgsManager<ArgsHandler>().Start(args);
var fileList = Args.Value("FL");
var dll = Args.Value("D");
var prototypeDLLs = Args.Value("PD").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries); 


var files = File.ReadAllLines(fileList);
var dirs = files.Select(f=>Path.GetDirectoryName(f)).ToList();
foreach (var dir in dirs)
{
    if (!Directory.Exists(dir))
    {
        Console.WriteLine($"This directory doesn't exist: {dir} ");
        return;
    }
    var wxsFiles = Directory.GetFiles(dir, $"*.wxs", SearchOption.AllDirectories);

    foreach (var wxsFile in wxsFiles)
    {
        var lines = File.ReadAllLines(wxsFile);
        var newFileLines = new List<string>();
        var i = 0;
        var line = lines[i];
        var doneWithWxsFile = false;
        // Write all lines before pattern found
        while (!line.Contains("Source=") || !prototypeDLLs.Any(dll => line.Contains($"){dll}")))
        {
            newFileLines.Add(line);
            i++;
            if (i < lines.Length)
            {
                line = lines[i];
            }
            else
            {
                doneWithWxsFile= true;
                break;
            }
        }

        if (doneWithWxsFile)
        {
            continue;
        }

        var projName = ParseForProjName(line);
        var leadingWhiteSpace = ParseForWhiteSpace(line);

        while (!line.Contains("/>"))
        {
            newFileLines.Add(line);
            line = lines[++i];
        }
        newFileLines.Add(line);
        line = lines[++i];
        newFileLines.Add(string.Format(Template, leadingWhiteSpace , dll, projName));


        //move to end of file
        while (i < lines.Length)
        {

            newFileLines.Add(line);
            i++;
            if (i < lines.Length)
                line = lines[i];
        }
        File.WriteAllLines(wxsFile, newFileLines);
            break;
    }

}

static string ParseForProjName(string line)
{
    var projNameStart = line.IndexOf("$(var.");
    var projNameEnd = line.IndexOf(".TargerDir", projNameStart);
    if (projNameStart != -1 && projNameEnd != -1)
    {

        return line.Substring(projNameStart + 6, projNameEnd - projNameStart - 6);
    }
    return "";
}

static string ParseForWhiteSpace(string line)
{
    return line.Substring(0, line.IndexOf('<'));

}