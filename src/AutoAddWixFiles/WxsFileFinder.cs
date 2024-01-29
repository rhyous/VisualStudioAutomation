using Rhyous.AutoAddDLLtoWXSFiles;
using System.Text.RegularExpressions;

namespace Rhyous.AutoAddDLLtoWXSFiles
{

    internal class WxsFileFinder : IWxsFileFinder
    {
        private readonly ISettings _settings;

        public WxsFileFinder(ISettings settings)
        {
            _settings = settings;
        }
        public IEnumerable<AddDetails> FindPotentialWxsFiles(IEnumerable<string> dirs)
        {
            var potentialWxsFiles = new Dictionary<string, AddDetails>();
            foreach (var dir in dirs)
            {
                potentialWxsFiles.Add(dir, null);
                if (!Directory.Exists(dir))
                {
                    Console.WriteLine($"This directory doesn't exist: {dir} ");
                    continue;
                }
                var wxsFiles = Directory.GetFiles(dir, $"*.wxs", SearchOption.AllDirectories);

                bool dllAlreadyExists = false;
                var addDetails = new AddDetails();
                foreach (var wxsFile in wxsFiles)
                {
                    var lines = File.ReadAllLines(wxsFile);
                    var newFileLines = new List<string>();
                    bool insideBlockComment = false;
                    bool foundLine = false;
                    for (var i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i];
                        // ignore comment
                        if (Regex.IsMatch(line, "^\\s*<!--") && !Regex.IsMatch(line, "^\\s*<!--.*-->$"))
                            insideBlockComment = true;
                        if (insideBlockComment || Regex.IsMatch(line, "^\\s*(\\\\|<!--)"))
                            continue;
                        if (line.Contains(_settings.Dll))
                        {
                            dllAlreadyExists = true;
                            break;
                        }
                        if (string.IsNullOrEmpty(addDetails.File) && line.Contains("Source=") && _settings.PrototypeDlls.Any(dll => line.Contains($"){dll}")))
                        {
                            foundLine = true;
                            addDetails.File = wxsFile;
                            addDetails.ProjectName = ParseForProjName(line);
                            addDetails.LeadingWhitespace = ParseForLeadingWhiteSpace(lines, i);
                        }
                        if (foundLine && lines[i].Contains("/>"))
                        {
                            foundLine = false;
                            addDetails.AfterLineNumber = i;
                        }
                    }
                    if (dllAlreadyExists)
                        break;
                }
                if (!string.IsNullOrEmpty(addDetails.File))
                    potentialWxsFiles[dir] = addDetails;

            }
            var dirsWithoutAPotentialWixFile = potentialWxsFiles.Where(kvp => kvp.Value is null).ToList();
            if (dirsWithoutAPotentialWixFile.Any())
            {
                Console.WriteLine($"Directories missing a potential wix file: {dirsWithoutAPotentialWixFile.Count}");
                foreach (var dir in dirsWithoutAPotentialWixFile)
                {
                    Console.WriteLine(dir);
                }
            }
            return potentialWxsFiles.Values;
        }
        static string ParseForProjName(string line)
        {
            var projNameStart = line.IndexOf("$(var.");
            var projNameEnd = line.IndexOf(".TargetDir", projNameStart);
            if (projNameStart != -1 && projNameEnd != -1)
            {
                return line.Substring(projNameStart + 6, projNameEnd - projNameStart - 6);
            }
            return "";
        }

        static string ParseForLeadingWhiteSpace(IList<string> lines, int i)
        {
            while (lines[i].IndexOf('<') == -1)
                i--;
            return lines[i].Substring(0, lines[i].IndexOf('<'));

        }
    }
}
