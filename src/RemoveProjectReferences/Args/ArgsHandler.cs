using Rhyous.SimpleArgs;

namespace RemoveProjectReferences.Arguments
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
                    Name = "FileList",
                    ShortName = "L",
                    Description = "The list of files to remove the ProjectReference from.",
                    Example = "{name}=c:\\Some\\Dir\\SomeList.txt",
                    IsRequired = true
                },                new Argument
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