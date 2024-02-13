using System;
using WixToolset.Dtf.WindowsInstaller;
using Rhyous.SimpleArgs;
using Rhyous.MsiFileInfoReader.Arguments;
using System.IO;


namespace Rhyous.MsiFileInfoReader
{
    class Program
    {
        static void Main(string[] args)
        {
            new ArgsManager<ArgsHandler>().Start(args);
            var tokenizedFolder = Args.Value("TF");
            var leftFolder = Args.Value("LF");
            var rightFolder = Args.Value("RF");
            var file = Args.Value("F");

            var leftFolderFullPath = string.Format(tokenizedFolder, leftFolder);
            var rightFolderFullPath = string.Format(tokenizedFolder, rightFolder);

            var msiFiles = Directory.GetFiles(leftFolderFullPath, "*.msi", SearchOption.AllDirectories);
            var msiFileFinder = new MSIFileFinder();
            foreach (var msiFile in msiFiles)
            {
                var leftMsiFile = msiFileFinder.Find(msiFile, file);
                if (leftMsiFile == null)
                { continue; }

                Console.WriteLine($"Side:  Left, Msi: {Path.GetFileName(msiFile)}, File: {file}, Version: {leftMsiFile.Version}, Directory: {leftMsiFile.Folder}");

                var rightFile = msiFile.Replace(leftFolderFullPath, rightFolderFullPath);
                var rightMsiFile = msiFileFinder.Find(rightFile, file);
                if (rightMsiFile != null)
                {
                    Console.WriteLine($"Side: Right, Msi: {Path.GetFileName(msiFile)}, File: {file}, Version: {rightMsiFile.Version.PadLeft(12, ' ')}, Directory: {rightMsiFile.Folder}");
                }
            }

        }
    }

    public class MsiFile
    {
        public Record FileRecord { get; set; }
        public string Version { get; set; }
        public Record ComponentRecord { get; set; }
        public string Folder { get; set; }
    }

    public class MSIFileFinder
    {
        public MsiFile Find(string msiPath, string fileNameToFind)
        {
            using (var database = new Database(msiPath, DatabaseOpenMode.ReadOnly))
            {
                // Query the File table to find the row for the specified file
                string fileQuery = $"SELECT `Component_`, `Version` FROM `File` WHERE `FileName` = '{fileNameToFind}' OR `FileName` = '{fileNameToFind}|'";
                using (var fileView = database.OpenView(fileQuery))
                {
                    fileView.Execute();
                    if (fileView.Fetch() is Record fileRecord)
                    {
                        string component = fileRecord["Component_"].ToString();

                        // Query the Component table to get the directory associated with the component
                        string componentQuery = $"SELECT `Directory_` FROM `Component` WHERE `Component` = '{component}'";
                        using (var componentView = database.OpenView(componentQuery))
                        {
                            componentView.Execute();
                            if (componentView.Fetch() is Record componentRecord)
                            {
                                return new MsiFile
                                {
                                    FileRecord = fileRecord,
                                    Version = fileRecord["Version"].ToString(),
                                    ComponentRecord = componentRecord,
                                    Folder = componentRecord["Directory_"].ToString(),
                                };

                                // You may need to resolve the directory to a full path by traversing the Directory table
                                // This example simply prints the directory ID
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
