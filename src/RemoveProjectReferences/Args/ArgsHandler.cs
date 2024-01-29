using Rhyous.SimpleArgs;

namespace Rhyous.RemoveProjectReferences.Arguments
{
    // Add this line of code to Main() in Program.cs
    //
    //   new ArgsManager<ArgsHandler>().Start(args);
    //

    /// <summary>
    /// A class that implements IArgumentsHandler where command line
    /// arguments are defined.
    /// </summary>
    public sealed class ArgsHandler : ArgsHandlerBase
    {
        public override void InitializeArguments(IArgsManager argsManager)
        {
            Arguments.AddRange(new List<Argument>
            {
                new Argument
                {
                    Name = "ProjectName",
                    ShortName = "P",
                    Description = "The string pattern to search for.",
                    Example = "{name}=PatternToSearch",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "FileExtension",
                    ShortName = "FE",
                    Description = "The file extension to find.",
                    Example = "{name}=csproj",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "SearchDirectory",
                    ShortName = "SD",
                    Description = "The search directory.",
                    Example = "{name}=c:\\Some\\Dir",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "DoNothing",
                    ShortName = "dn",
                    Description = "Instead of doing anything, it will just list what it would have changed.",
                    Example = "{name}=true",
                    IsRequired = false,
                    DefaultValue = "false"
                },
                new Argument
                {
                    Name = "TFSCheckout",
                    ShortName = "tfc",
                    Description = "Check out the files with TFS.",
                    Example = "{name}=true",
                    IsRequired = false,
                    DefaultValue = "false"
                },
                new Argument
                {
                    Name = "TFdotExePath",
                    ShortName = "tf",
                    Description = "Check out the files with TFS.",
                    Example = "{name}=true",
                    IsRequired = false,
                    DefaultValue = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Enterprise\\Common7\\IDE\\CommonExtensions\\Microsoft\\TeamFoundation\\Team Explorer\\TF.exe",
                    CustomValidation = File.Exists
                },
                new Argument
                {
                    Name = "ExcludeDirectories",
                    ShortName = "ed",
                    Description = "Directories to skip, semicolon separated.",
                    Example = "{name}=c:\\dev\folderToSkip",
                    IsRequired = false
                },
                new ConfigFileArgument(argsManager) // This is a special Argument type to allow for args in a file
            });
        }

        public override void HandleArgs(IReadArgs inArgsHandler)
        {
            base.HandleArgs(inArgsHandler);
            Console.WriteLine("I handled the args!!!");
        }
    }
}